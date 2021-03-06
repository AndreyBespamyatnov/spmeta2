﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPMeta2.Definitions;
using SPMeta2.Models;

namespace SPMeta2.Syntax.Default.Extensions
{
    public static class ModelNodeExtensions
    {
        public static ModelNode AddDefinitionNode(this ModelNode node, DefinitionBase definition)
        {
            return AddDefinitionNode(node, definition, null);

        }

        public static ModelNode AddDefinitionNode(this ModelNode node,
            DefinitionBase definition,
            Action<ModelNode> action)
        {
            var contentTypeModelNode = new ModelNode { Value = definition };

            node.ChildModels.Add(contentTypeModelNode);

            if (action != null)
                action(contentTypeModelNode);

            return node;
        }
    }
}
