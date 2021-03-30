using System;

namespace FindTheIdol.Utilities.Attributes
{
    //Marking code as evil (following Jon Skeet guideline :) )
    //Especially when they try to use GetArchetype (massive overhead, but required for serialization later on (save mechanism))
    [AttributeUsage(AttributeTargets.All)]
    public class EvilAttribute : Attribute
    {
        private string message;

        public EvilAttribute(string message)
        {
            this.message = message;
        }
    }
}
