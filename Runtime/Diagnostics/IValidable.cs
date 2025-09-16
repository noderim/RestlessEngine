namespace RestlessEngine.Diagnostics
{
    public interface IValidable
    {
        /// <summary>
        /// Function to implement by inheriting classes. Dont call this function directly, use ValidateUtility.Validate(this) instead.
        /// </summary>
        public virtual bool Validation()
        {
            return true;
        }
    }
}
