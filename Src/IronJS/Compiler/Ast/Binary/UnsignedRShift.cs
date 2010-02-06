﻿using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class UnsignedRShift : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }

        public UnsignedRShift(INode left, INode right, ITree node)
            : base(NodeType.UnsignedRightShift, node)
        {
            Left = left;
            Right = right;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Integer;
            }
        }

        public override INode Analyze(Function astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);

            IfIdentifierAssignedFrom(Left, Right);
            IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}