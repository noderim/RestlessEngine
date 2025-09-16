namespace RestlessEngine.Diagnostics
{
    public static class ValidateUtility
    {
        public static bool Validate(IValidable validableObject)
        {
            //LogManager.Log("Validating " + validableObject.GetType().Name, LogTag.Validation);

            bool validation = validableObject.Validation();

            if (!validation)
            {
                LogManager.LogWarning("Validation failed - " + validableObject.GetType().Name, LogTag.Validation);
            }
            else
            {
                LogManager.Log("Validation passed - " + validableObject.GetType().Name, LogTag.Validation);
            }
            return validation;
        }

        public static bool Validate(IValidable[] validableObjects)
        {
            bool validation = true;

            foreach (IValidable validableObject in validableObjects)
            {
                validation &= Validate(validableObject);
            }

            return validation;
        }

        public static bool ValidateCondition(bool condition, string message = "")
        {
            if (condition == false)
            {
                LogManager.LogWarning("Condition not met warning - " + message, LogTag.Validation);
            }
            return condition;
        }

        public static bool ValidateReference<T>(T reference)
        {
            if (reference == null)
            {
                LogManager.LogWarning($"Empty Refrence warning - {reference.GetType()} cannot be null", LogTag.Validation);
                return false;
            }
            return true;
        }

        public static bool ValidateReference<T>(T reference, string message, bool asAdditional = true)
        {
            if (reference == null)
            {
                if (asAdditional)
                {
                    LogManager.LogWarning($"Empty Refrence warning - {reference.GetType()} cannot be null\n{message}", LogTag.Validation);
                }
                else
                {
                    LogManager.LogWarning(message, LogTag.Validation);
                }
                return false;
            }
            return true;
        }
    }
}
