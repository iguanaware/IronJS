﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class BlockNode : Node
    {
        public List<INode> Nodes { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public BlockNode(List<INode> nodes, ITree node)
            : base(NodeType.Block, node)
        {
            Nodes = nodes;
            IsEmpty = nodes.Count == 0;
        }

        public override Et Generate2(EtGenerator etgen)
        {
            if(Nodes.Count > 0)
                return Et.Block(
                    Nodes.Select(x => x.Generate2(etgen))
                );

            return AstUtils.Empty();
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            var nodes = new List<INode>();

            foreach (var node in Nodes)
                nodes.Add(node.Optimize(astopt));

            Nodes = nodes;
            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            if (Nodes.Count == 0)
                return Et.Default(typeof(object));

            return Et.Block(
                Nodes.Select(x => x.Generate(etgen))
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.Append(indentStr + "(" + NodeType + "");

            if (!IsEmpty)
            {
                writer.AppendLine();

                foreach (var node in Nodes)
                    node.Print(writer, indent + 1);

                writer.AppendLine(indentStr + ")");
            }
            else
            {
                writer.AppendLine(" empty)");
            }
        }

    }
}
