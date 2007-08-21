using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace CodeReformatter
{
    [Serializable]
    public class Settings
    {
        private Boolean NEWLINE_AFTER_CLASS_ = true;   // insert a newline after class definition (before the '{')
        private Boolean NEWLINE_AFTER_METHOD_ = true;   // insert a newline after a method definition (before the '{')
        private Boolean NEWLINE_AFTER_CONDITION_ = true;   // insert newline after if, for, while...
        private Boolean NEWLINE_BEFORE_ELSE_ = true;   // insert newline after 'else'
        private Boolean SPACE_BETWEEN_TYPE_ = false;
        private Boolean SPACE_BETWEEN_ASSIGN_ = true;
        private Boolean SPACE_BETWEEN_METHOD_CALL_ = false;
        private Boolean SPACE_BETWEEN_OPERATORS_ = true;
        private Boolean SPACE_BETWEEN_ARGUMENTS_ = true;

        [Description("Insert a newline after class/package definition"), DefaultValue(true)]
        public Boolean NEWLINE_AFTER_CLASS
        {
            get { return this.NEWLINE_AFTER_CLASS_; }
            set { this.NEWLINE_AFTER_CLASS_ = value; }
        }

        [Description("Insert a newline after method definition"), DefaultValue(true)]
        public Boolean NEWLINE_AFTER_METHOD
        {
            get { return this.NEWLINE_AFTER_METHOD_; }
            set { this.NEWLINE_AFTER_METHOD_ = value; }
        }

        [Description("Insert a newline after a loop (for, if, while..)"), DefaultValue(true)]
        public Boolean NEWLINE_AFTER_CONDITION
        {
            get { return this.NEWLINE_AFTER_CONDITION_; }
            set { this.NEWLINE_AFTER_CONDITION_ = value; }
        }

        [Description("Insert a newline before 'else'"), DefaultValue(true)]
        public Boolean NEWLINE_BEFORE_ELSE
        {
            get { return this.NEWLINE_BEFORE_ELSE_; }
            set { this.NEWLINE_BEFORE_ELSE_ = value; }
        }

        [Description("Insert between ':' type block"), DefaultValue(true)]
        public Boolean SPACE_BETWEEN_TYPE
        {
            get { return this.SPACE_BETWEEN_TYPE_; }
            set { this.SPACE_BETWEEN_TYPE_ = value; }
        }

        [Description("Insert a space between '='"), DefaultValue(true)]
        public Boolean SPACE_BETWEEN_ASSIGN
        {
            get { return this.SPACE_BETWEEN_ASSIGN_; }
            set { this.SPACE_BETWEEN_ASSIGN_ = value; }
        }

        [Description("Insert a space before and after method call parameter list '()'"), DefaultValue(true)]
        public Boolean SPACE_BETWEEN_METHOD_CALL
        {
            get { return this.SPACE_BETWEEN_METHOD_CALL_; }
            set { this.SPACE_BETWEEN_METHOD_CALL_ = value; }
        }

        [Description("Insert a space between operators"), DefaultValue(true)]
        public Boolean SPACE_BETWEEN_OPERATORS
        {
            get { return this.SPACE_BETWEEN_OPERATORS_; }
            set { this.SPACE_BETWEEN_OPERATORS_ = value; }
        }

        [Description("Insert a space after each argument, parameter"), DefaultValue(true)]
        public Boolean SPACE_BETWEEN_ARGUMENTS
        {
            get { return this.SPACE_BETWEEN_ARGUMENTS_; }
            set { this.SPACE_BETWEEN_ARGUMENTS_ = value; }
        }
    }

}
