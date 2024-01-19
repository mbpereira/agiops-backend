using Domain.Validation;

namespace Domain.Shared
{

    public abstract class BaseEntity<T>
    {
        protected readonly Validator<T> Validator;
        public int Id { get; protected set; }

        public BaseEntity()
        {
            Validator = new Validator<T>();    
        }

        public abstract ValidationResult Validate();
    }
}
