﻿namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators
open IronJS.Support.CustomOperators

module internal Unary =
  
  //----------------------------------------------------------------------------
  // 11.3
  let postIncDec (ctx:Ctx) (ast:Ast.Tree) op =
    let expr = ctx.Compile ast

    let incrementExpr = 
      ctx.Compile(
        Ast.Assign(ast, 
          Ast.Binary(op, 
            Ast.Convert(TypeTags.Number, ast), 
            Ast.Number 1.0)))

    Dlr.blockTmp expr.Type (fun tmp ->
     [
      Dlr.assign tmp expr; incrementExpr
      TC.ToNumber(tmp :> Dlr.Expr) //HACK
     ] |> Seq.ofList)

  //----------------------------------------------------------------------------
  // 11.3.1
  let postIncrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Add

  //----------------------------------------------------------------------------
  // 11.3.2
  let postDecrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Sub
  
  //----------------------------------------------------------------------------
  // 11.4.1 delete
  let deleteIndex (ctx:Ctx) object' index =
    (Utils.ensureObject ctx object'
      (fun x -> Dlr.call x "Delete" [index])
      (fun x -> Dlr.false'))
    
  let deleteProperty (ctx:Ctx) object' name =
    (Utils.ensureObject ctx object'
      (fun x -> Dlr.call x "Delete" [!!!name])
      (fun x -> Dlr.false'))
    
  let deleteIdentifier (ctx:Ctx) name =
    if ctx.DynamicLookup then
      let args = [ctx.Parameters.DynamicScope :> Dlr.Expr; ctx.Globals; Dlr.const' name]
      Dlr.callStaticT<DynamicScopeHelpers> "Delete" args

    elif name = "arguments" && ctx.Scope |> Ast.Utils.isFunction then
      !!!false

    elif Identifier.isGlobal ctx name then
      deleteProperty ctx ctx.Globals name

    else
      !!!false

  let delete (ctx:Ctx) tree =
    match tree with
    | Ast.Identifier name -> 
      deleteIdentifier ctx name

    | Ast.Index(object', index) ->
      let index = Utils.compileIndex ctx index
      let object' = ctx.Compile object'
      deleteIndex ctx object' index

    | Ast.Property(object', name) ->
      let object' = ctx.Compile object'
      deleteProperty ctx object' name

    | Ast.This ->
      !!!false

    | _ -> !!!true

  //----------------------------------------------------------------------------
  // 11.4.2
  let void' (ctx:Ctx) ast =
    Dlr.blockSimple [ctx.Compile ast; Utils.Constants.undefined]
        
  //----------------------------------------------------------------------------
  // 11.4.3
  let typeOf (ctx:Ctx) (ast:Ast.Tree) = 
    match ast with
    | Ast.Identifier name ->

      let expr =

        if name |> Identifier.isGlobal ctx then 
          
          // We have to use the proper 
          // .Get method of the global object
          // here to make sure we don't throw
          // a reference exception when a non
          // defined variable is accessed
          Object.Property.get !!!name ctx.Globals

        else 
          ast |> ctx.Compile

      expr |> Utils.box |> Operators.typeOf
      
    | _ ->
      ast |> ctx.Compile |> Utils.box |> Operators.typeOf
      
  //----------------------------------------------------------------------------
  // 11.4.4, 11.4.5
  let preIncDec (ctx:Ctx) ast op =
    let incrementExpr = 
      ctx.Compile(
        Ast.Assign(ast, 
          Ast.Binary(op, 
            Ast.Convert(TypeTags.Number, ast), 
            Ast.Number 1.0)))

    Dlr.blockSimple [incrementExpr; ctx.Compile ast]
    
  let increment ctx ast = preIncDec ctx ast Ast.BinaryOp.Add
  let decrement ctx ast = preIncDec ctx ast Ast.BinaryOp.Sub
  
  //----------------------------------------------------------------------------
  // 11.4.6
  let plus (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "plus" [ctx.Compile ast |> Utils.box]

  //----------------------------------------------------------------------------
  // 11.4.7
  let minus (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "minus" [ctx.Compile ast |> Utils.box]
  
  //----------------------------------------------------------------------------
  // 11.4.8
  let complement (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "bitCmpl" [ctx.Compile ast |> Utils.box]

  //----------------------------------------------------------------------------
  // 11.4.9
  let not (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "not" [ctx.Compile ast |> Utils.box]
    
  //----------------------------------------------------------------------------
  let convert (ctx:Ctx) (tag:uint32) (ast:Ast.Tree) =
    match tag with
    | TypeTags.Number ->
      Dlr.callStaticT<TypeConverter> "ToNumber" [ctx.Compile ast]

    | _ -> failwith "Que?"

  /// 
  let compile ctx op ast =
    match op with
    | Ast.UnaryOp.Delete -> delete ctx ast
    | Ast.UnaryOp.TypeOf -> typeOf ctx ast
    | Ast.UnaryOp.Void -> void' ctx ast
    | Ast.UnaryOp.Inc -> increment ctx ast
    | Ast.UnaryOp.Dec -> decrement ctx ast
    | Ast.UnaryOp.PostInc -> postIncrement ctx ast
    | Ast.UnaryOp.PostDec -> postDecrement ctx ast
    | Ast.UnaryOp.BitCmpl -> complement ctx ast
    | Ast.UnaryOp.Not -> not ctx ast
    | Ast.UnaryOp.Plus -> plus ctx ast
    | Ast.UnaryOp.Minus -> minus ctx ast
    | _ -> failwithf "Invalid unary op %A" op

module internal Binary = 
  
  ///
  let private toNumber (expr:Dlr.Expr) =
    match TypeTag.OfType(expr.Type) with
    | TypeTags.Number -> expr
    | TypeTags.Box ->
      Utils.tempBlock expr (fun tmp ->
        [
          Dlr.ternary 
            (Utils.Box.isNumber tmp) 
            (Utils.Box.unboxNumber tmp)
            (TC.ToNumber(tmp))
        ]
      )

    | _ -> 
      TC.ToNumber(expr)

  ///
  let private toUInt32 (expr:Dlr.Expr) =
    expr |> toNumber |> Dlr.castT<uint32>

  ///
  let private toInt32 (expr:Dlr.Expr) =
    expr |> toUInt32 |> Dlr.castT<int32>

  ///
  let private numericOperator op l r : Dlr.Expr =
    let tempL = Dlr.paramT<double> "left"
    let tempR = Dlr.paramT<double> "right"
    let l = toNumber l
    let r = toNumber r
    let variables = ref []
    let body = ref []

    let realL = 
      if l |> Dlr.isStatic then l 
      else 
        variables := [tempL]
        body := [tempL .= l]
        tempL :> Dlr.Expr

    let realR = 
      if r |> Dlr.isStatic then r 
      else 
        variables := tempR :: !variables
        body := !body @ [tempR .= r]
        tempR :> Dlr.Expr

    Dlr.block !variables (!body @ [op realL realR])

  ///
  let private bitOperator op l r : Dlr.Expr = 
    op (toInt32 l) (toInt32 r) |> Dlr.castT<double>

  ///
  let private bitShiftOperator op convert l r : Dlr.Expr = 
    let r = ((r |> toUInt32) .& (!!!0x1Fu)) |> Dlr.castT<int>
    op (convert l) r |> Dlr.castT<double>

  ///
  let compileExpr (ctx:Ctx) op (l:Dlr.Expr) r =
    match op with
    | Ast.BinaryOp.Add -> Operators.add(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.Sub -> numericOperator Dlr.sub   l r
    | Ast.BinaryOp.Div -> numericOperator Dlr.div   l r
    | Ast.BinaryOp.Mul -> numericOperator Dlr.mul   l r
    | Ast.BinaryOp.Mod -> numericOperator Dlr.mod'  l r

    | Ast.BinaryOp.BitAnd -> bitOperator Dlr.bAnd' l r
    | Ast.BinaryOp.BitOr -> bitOperator Dlr.bOr' l r
    | Ast.BinaryOp.BitXor -> bitOperator Dlr.xor l r
    | Ast.BinaryOp.BitShiftLeft -> bitShiftOperator Dlr.lhs toInt32 l r
    | Ast.BinaryOp.BitShiftRight -> bitShiftOperator Dlr.rhs toInt32 l r
    | Ast.BinaryOp.BitUShiftRight -> bitShiftOperator Dlr.rhs toUInt32 l r

    | Ast.BinaryOp.Eq -> Operators.eq(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.NotEq -> Operators.notEq(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.Same -> Operators.same(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.NotSame -> Operators.notSame(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.Lt -> Operators.lt(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.LtEq -> Operators.ltEq(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.Gt -> Operators.gt(l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.GtEq -> Operators.gtEq(l |> Utils.box, r |> Utils.box)

    | Ast.BinaryOp.In -> Operators.in'(ctx.Env, l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.InstanceOf -> Operators.instanceOf(ctx.Env, l |> Utils.box, r |> Utils.box)

    | _ -> failwithf "Invalid BinaryOp %A" op
    
  /// Implements compilation for: 
  let compile (ctx:Ctx) op left right =

    match op with
    | Ast.BinaryOp.Or -> 
      let l = left |> ctx.Compile
      let l_tmp = Dlr.param "~left" l.Type

      Dlr.block [l_tmp] [
        l_tmp .= l
        (Dlr.ternary 
          (Dlr.callStaticT<TC> "ToBoolean" [l_tmp])
          (Utils.box l_tmp)
          (Utils.box (right |> ctx.Compile))
        )
      ]

    | Ast.BinaryOp.And -> 
      let l = left |> ctx.Compile
      let l_tmp = Dlr.param "~left" l.Type

      Dlr.block [l_tmp] [
        l_tmp .= l
        (Dlr.ternary 
          (Dlr.callStaticT<TC> "ToBoolean" [l_tmp])
          (Utils.box (right |> ctx.Compile))
          (Utils.box l_tmp)
        )
      ]

    | _ ->
      let l = left |> ctx.Compile
      let r = right |> ctx.Compile
      compileExpr ctx op l r

  /// Implements compilation for 11.13.1 assignment operator =
  let assign (ctx:Ctx) ltree rtree =
    let value = ctx.Compile rtree

    match ltree with
    //Variable assignment: foo = 1;
    | Ast.Identifier(name) -> 
      Identifier.setValue ctx name value

    //Property assignment: foo.bar = 1;
    | Ast.Property(object', name) -> 
      Utils.tempBlock value (fun value ->
        let object' = object' |> ctx.Compile
        let ifObj = Object.Property.put !!!name value
        let ifClr _ = value
        [Utils.ensureObject ctx object' ifObj ifClr]
      )

    //Index assignemnt: foo[0] = "bar";
    | Ast.Index(object', index) -> 
      Utils.tempBlock value (fun value ->
        let object' = object' |> ctx.Compile
        let index = Utils.compileIndex ctx index
        let ifObj = Object.Index.put index value
        let ifClr _ = value
        [Utils.ensureObject ctx object' ifObj ifClr]
      )

    | _ -> failwithf "Failed to compile assign for: %A" ltree
    
  /// Implements compilation for 
  let compoundAssign (ctx:Ctx) op ltree rtree =
    match ltree with
    | Ast.Index(obj, idx) ->
      let obj = ctx $ Context.compile obj
      let idx = ctx $ Context.compile idx
      let tmp = Dlr.param (Dlr.tmpName()) idx.Type
      let ltree = Ast.Index(Ast.DlrExpr obj, Ast.DlrExpr tmp)

      Dlr.block [tmp] [
        tmp .= idx
        assign ctx ltree (Ast.Binary(op, ltree,rtree))
      ]
      
    | _ ->
      assign ctx ltree (Ast.Binary(op, ltree, rtree))