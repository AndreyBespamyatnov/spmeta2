﻿using Microsoft.SharePoint.Client;

namespace SPMeta2.CSOM.ModelHosts
{
    public class FolderModelHost 
    {
        #region properties

        public List CurrentList { get; set; }

        public ListItem CurrentListItem { get; set; }
        public Folder CurrentLibraryFolder { get; set; }

        public Web CurrentWeb { get; set; }
        public Folder CurrentWebFolder { get; set; }

        //public Folder Folder { get; set; }
        //public List List { get; set; }
        //public Web Web { get; set; }

        #endregion
    }
}
