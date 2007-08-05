using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FlashAPI.Controls.TreeView
{
    class TreeAPINode : TreeNode
    {
        public String Action;
        public bool IsActionNode;

        public TreeAPINode( String name )
            : base(name)
        {
            Action = null;
            this.IsActionNode = false;
        }

        public TreeAPINode( String name, String nodeAction )
            : base(name)
        {
            Action = nodeAction;
            this.IsActionNode = true;
        }
    }
}
