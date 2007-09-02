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
        private Boolean newline_after_method = true;   // insert a newline after a method definition (before the '{')
        private Boolean newline_after_condition = true;   // insert newline after if, for, while...
        private Boolean newline_before_else = true;   // insert newline after 'else'
        private Boolean newline_between_fields = true;
        private Boolean space_between_type = false;
        private Boolean space_between_assign = true;
        private Boolean space_before_method_call = false;
        private Boolean space_between_operators = true;
        private Boolean space_between_arguments = true;

        [Description("Insert a newline after method definition"), DefaultValue(true)]
        public Boolean NewlineAfterMethod
        {
            get { return this.newline_after_method; }
            set { this.newline_after_method = value; }
        }

        [Description("Insert a newline after a loop (for, if, while..)"), DefaultValue(true)]
        public Boolean NewlineAfterCondition
        {
            get { return this.newline_after_condition; }
            set { this.newline_after_condition = value; }
        }

        [Description("Insert a newline before 'else'"), DefaultValue(true)]
        public Boolean NewlineBeforeElse
        {
            get { return this.newline_before_else; }
            set { this.newline_before_else = value; }
        }

        [Description("Insert between ':' type block"), DefaultValue(true)]
        public Boolean SpaceBetweenType
        {
            get { return this.space_between_type; }
            set { this.space_between_type = value; }
        }

        [Description("Insert a space between '='"), DefaultValue(true)]
        public Boolean SpaceBetweenAssign
        {
            get { return this.space_between_assign; }
            set { this.space_between_assign = value; }
        }

        [Description("Insert a space before and after method call parameter list '()'"), DefaultValue(true)]
        public Boolean SpaceBeforeMethodCall
        {
            get { return this.space_before_method_call; }
            set { this.space_before_method_call = value; }
        }

        [Description("Insert a space between operators"), DefaultValue(true)]
        public Boolean SpaceBetweenOperators
        {
            get { return this.space_between_operators; }
            set { this.space_between_operators = value; }
        }

        [Description("Insert a space after each argument, parameter"), DefaultValue(true)]
        public Boolean SpaceBetweenArguments
        {
            get { return this.space_between_arguments; }
            set { this.space_between_arguments = value; }
        }

        [Description("Insert a newline after each object field, if any"), DefaultValue(true)]
        public Boolean NewlineBetweenFields
        {
            get { return this.newline_between_fields; }
            set { this.newline_between_fields = value; }
        }
    }

}
