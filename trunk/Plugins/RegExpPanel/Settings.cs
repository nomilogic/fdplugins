using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

namespace RegExpPanel
{
    [Serializable]
    public class Settings
    {
        private String  lastMatchText          = String.Empty;
        private String  lastReplacePatternText = String.Empty;
        private String  lastReplaceReplText    = String.Empty;

        private string[] library = new string[] { @"Credit Card (Major Cards):^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6011[0-9]{14}|3(?:0[0-5]|[68][0-9])[0-9]{11}|3[47][0-9]{13})$",
                                                    @"Date (d/m/yy and dd/mm/yyyy):\b(0?[1-9]|[12][0-9]|3[01])[- /.](0?[1-9]|1[012])[- /.](19|20)?[0-9]{2}\b",
                                                    @"Date (dd/mm/yyyy):(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)[0-9]{2}",
                                                    @"Date (m/d/yy and mm/dd/yyyy):\b(0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2}\b",
                                                    @"Date (mm/dd/yyyy):(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)[0-9]{2}",
                                                    @"Email Address:\b[A-Z0-9._%-]+@[A-Z0-9._%-]+\.[A-Z]{2,4}\b",
                                                    @"HTML (Tag):</?[a-z][a-z0-9]*[^<>]*>",
                                                    @"IP Address:\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b",
                                                    @"Currency amount:\b[0-9]{1,3}(?:,?[0-9]{3})*\.[0-9]{2}\b",
                                                    @"Password complexity (6 or more characters):\A(?=[-_a-zA-Z0-9]*?[A-Z])(?=[-_a-zA-Z0-9]*?[a-z])(?=[-_a-zA-Z0-9]*?[0-9])\S{6,}\z",
                                                    @"Programming /* comment */:\/\*.*?\*\/",
                                                    @"Programming // comment	//:.*$",
                                                    @"Social Security Number (US):\b[0-9]{3}-[0-9]{2}-[0-9]{4}\b",
                                                    @"URL Path:\b(?<protocol>https?|ftp)://(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)?(?<parameters>\?[-A-Z0-9+&@#/%=~_|!:,.;]*)?",
                                                    @"URL:\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]",
                                                    @"ZIP Code (US):\b[0-9]{5}(?:-[0-9]{4})?\b", };

        private Boolean matchFindAll    = false;
        private Boolean matchIgnoreCase = false;
        private Boolean matchMultiline  = false;
        private Boolean matchDotAll     = false;
        private Boolean matchVerbose    = false;

        private Boolean replaceIgnoreCase = false;
        private Boolean replaceMultiline  = false;
        private Boolean replaceDotAll     = false;
        private Boolean replaceVerbose    = false;
        private Boolean replaceFindAll    = false;

        private Keys shortcut = Keys.Control | Keys.Alt | Keys.R;
        private Font patternFont = new System.Drawing.Font("Courier New", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


        [Description("Library Patterns. Use ':' to separate label from the pattern (eg. test:([a-z]+)"), 
        DisplayName("Library Patterns"),
        DefaultValue(new string[] { @"Credit Card (Major Cards):^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6011[0-9]{14}|3(?:0[0-5]|[68][0-9])[0-9]{11}|3[47][0-9]{13})$",
                                                    @"Date (d/m/yy and dd/mm/yyyy):\b(0?[1-9]|[12][0-9]|3[01])[- /.](0?[1-9]|1[012])[- /.](19|20)?[0-9]{2}\b",
                                                    @"Date (dd/mm/yyyy):(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)[0-9]{2}",
                                                    @"Date (m/d/yy and mm/dd/yyyy):\b(0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2}\b",
                                                    @"Date (mm/dd/yyyy):(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)[0-9]{2}",
                                                    @"Email Address:\b[A-Z0-9._%-]+@[A-Z0-9._%-]+\.[A-Z]{2,4}\b",
                                                    @"HTML (Tag):</?[a-z][a-z0-9]*[^<>]*>",
                                                    @"IP Address:\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b",
                                                    @"Currency amount:\b[0-9]{1,3}(?:,?[0-9]{3})*\.[0-9]{2}\b",
                                                    @"Password complexity (6 or more characters):\A(?=[-_a-zA-Z0-9]*?[A-Z])(?=[-_a-zA-Z0-9]*?[a-z])(?=[-_a-zA-Z0-9]*?[0-9])\S{6,}\z",
                                                    @"Programming /* comment */:\/\*.*?\*\/",
                                                    @"Programming // comment	//:.*$",
                                                    @"Social Security Number (US):\b[0-9]{3}-[0-9]{2}-[0-9]{4}\b",
                                                    @"URL Path:\b(?<protocol>https?|ftp)://(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)?(?<parameters>\?[-A-Z0-9+&@#/%=~_|!:,.;]*)?",
                                                    @"URL:\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]",
                                                    @"ZIP Code (US):\b[0-9]{5}(?:-[0-9]{4})?\b", })]
        public string[] Library
        {
            get { return this.library; }
            set { this.library = value; }
        }


        /// <summary> 
        /// Get and sets the Shortcut
        /// </summary>
        [Description("Panel Shortcut."), DefaultValue(Keys.Control | Keys.Alt |Keys.R)]
        public Keys Shortcut
        {
            get { return this.shortcut; }
            set { this.shortcut = value; }
        }

        [Description("Pattern Font")]
        public Font PatternFont
        {
            get { return this.patternFont; }
            set { this.patternFont = value; }
        }

        public String LastMatchTextPattern
        {
            get { return this.lastMatchText; }
            set { this.lastMatchText = value; }
        }

        public String LastReplaceTextPattern
        {
            get { return this.lastReplacePatternText; }
            set { this.lastReplacePatternText = value; }
        }

        public String LastReplaceTextReplacement
        {
            get { return this.lastReplaceReplText; }
            set { this.lastReplaceReplText = value; }
        }


        public Boolean MatchFindAll
        {
            get { return this.matchFindAll; }
            set { this.matchFindAll = value; }
        }

        public Boolean MatchIgnoreCase
        {
            get { return this.matchIgnoreCase; }
            set { this.matchIgnoreCase = value; }
        }

        public Boolean MatchMultiline
        {
            get { return this.matchMultiline; }
            set { this.matchMultiline = value; }
        }

        public Boolean MatchDotAll
        {
            get { return this.matchDotAll; }
            set { this.matchDotAll = value; }
        }

        public Boolean MatchVerbose
        {
            get { return this.matchVerbose; }
            set { this.matchVerbose = value; }
        }


        public Boolean ReplaceIgnoreCase
        {
            get { return this.replaceIgnoreCase; }
            set { this.replaceIgnoreCase = value; }
        }

        public Boolean ReplaceMultiline
        {
            get { return this.replaceMultiline; }
            set { this.replaceMultiline = value; }
        }

        public Boolean ReplaceDotAll
        {
            get { return this.replaceDotAll; }
            set { this.replaceDotAll = value; }
        }

        public Boolean ReplaceVerbose
        {
            get { return this.replaceVerbose; }
            set { this.replaceVerbose = value; }
        }

        public Boolean ReplaceFindAll
        {
            get { return this.replaceFindAll; }
            set { this.replaceFindAll = value; }
        }
    }
}
