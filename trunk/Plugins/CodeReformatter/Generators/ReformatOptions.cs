using System;
using System.Collections.Generic;
using System.Text;

namespace CodeReformatter.Generators
{
    public class ReformatOptions
    {
        public Boolean NewlineAfterMethod = true; // insert a newline after a method definition (before the '{')
        public Boolean NewlineAfterCondition = false; // insert newline after if, for, while...
        public Boolean NewlineBeforeElse = false; // insert newline after 'else'
        public Boolean NewlineBetweenFields = true; // separate object literals by newline
        public Boolean SpaceBetweenType = false; // Separate type declaration with a space
        public Boolean SpaceBetweenAssign = true; // Seaparate assignment expression with a space
        public Boolean SpaceBeforeMethodDef = false; // Add a space before a method call
        public Boolean SpaceBetweenOperators = true; // Space between any operator
        public Boolean SpaceBetweenArguments = true; // Space between arguments

        public ReformatOptions(Settings settingObject)
        {
            NewlineAfterMethod = settingObject.NewlineAfterMethod;
            NewlineAfterCondition = settingObject.NewlineAfterCondition;
            NewlineBeforeElse = settingObject.NewlineBeforeElse;
            NewlineBetweenFields = settingObject.NewlineBetweenFields;
            SpaceBetweenType = settingObject.SpaceBetweenType;
            SpaceBetweenAssign = settingObject.SpaceBetweenAssign;
            SpaceBeforeMethodDef = settingObject.SpaceBeforeMethodCall;
            SpaceBetweenOperators = settingObject.SpaceBetweenOperators;
            SpaceBetweenArguments = settingObject.SpaceBetweenArguments;
        }

        public ReformatOptions()
        {
        }
    }
}
