﻿using SPMeta2.Definitions;
using SPMeta2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMeta2.Extensions
{
    public static class ModelNodeExtensions
    {
        #region static

        public static ModelNode WithNodes<T>(this ModelNode model, T definition)
            where T : DefinitionBase
        {
            return WithNode(model, definition);

        }

        public static ModelNode WithNodes<T>(this ModelNode model, T definition, Action<ModelNode> action)
           where T : DefinitionBase
        {
            var nodes = FindNodes(model, definition);

            if (action != null)
            {
                foreach (var node in nodes)
                {
                    action(node);
                }
            }

            return model;
        }

        public static ModelNode WithNode<T>(this ModelNode model, T definition)
            where T : DefinitionBase
        {
            return WithNode(model, definition);

        }

        public static ModelNode WithNode<T>(this ModelNode model, T definition, Action<ModelNode> action)
            where T : DefinitionBase
        {
            var node = FindFirstOrDefaultNode(model, definition);

            if (node != null && action != null)
            {
                action(node);
            }

            return model;
        }

        public static ModelNode FindFirstOrDefaultNode(this ModelNode model, DefinitionBase definition)
        {
            return FindNodes(model, definition).FirstOrDefault();
        }

        public static List<ModelNode> FindNodes(this ModelNode model, DefinitionBase definition)
        {
            var result = new List<ModelNode>();

            if (model.Value == definition)
                result.Add(model);

            foreach (var node in model.ChildModels)
            {
                var tmpNodes = FindNodes(node, definition);

                foreach (var tmpNode in tmpNodes)
                {
                    if (!result.Contains(tmpNode))
                        result.Add(tmpNode);
                }
            }

            return result;
        }

        #endregion
    }
}
