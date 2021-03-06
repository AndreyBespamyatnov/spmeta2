﻿using System;

namespace SPMeta2.Definitions
{
    public abstract class PageDefinitionBase : DefinitionBase
    {
        #region contructors

        public PageDefinitionBase()
        {
            NeedOverride = true;
        }

        #endregion

        #region properties

        public string Title { get; set; }
        public string FileName { get; set; }

        public bool NeedOverride { get; set; }

        #endregion

        #region methods

        public override string ToString()
        {
            return string.Format("Title:[{0}] FileName:[{1}]", Title, FileName);
        }

        #endregion
    }
}
