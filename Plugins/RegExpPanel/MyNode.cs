using System;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree;

namespace RegExpPanel
{
    public class MyNode : Node
    {
        private String _group;
        private String _span;
        private int[] _ispan;

        public MyNode(String group, String span, String text)
            : base(text)
        {
            _group = group;
            _span = span;
        }

        public String Group
        {
            get { return this._group; }
            set { this._group = value; }
        }

        public String Span
        {
            get { return this._span; }
            set { this._span = value; }
        }

        public int[] Spans
        {
            set { this._ispan = value; }
            get { return this._ispan; }
        }

    }
}
