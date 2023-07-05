using Microsoft.Net.Http.Headers;
using System.Reflection;

namespace HorusAPI.Helpers
{
    /// <summary>
    /// A generic builder
    /// </summary>
    /// <typeparam name="T">The type of the object to build</typeparam>
    public class Builder<T> where T: new()
    {
        T value;

        public Builder() {
            value = new T();
        }

        /// <summary>
        /// Returns the vuilt object
        /// </summary>
        /// <returns>The built object</returns>
        public T Build()
        {
            return value;
        }

        /// <summary>
        /// Sets the property to the value. Note that this expects a setter function!
        /// </summary>
        /// <typeparam name="V">The type of the field</typeparam>
        /// <param name="value">The value of the field</param>
        /// <param name="name">The name of the field</param>
        /// <returns>the builder</returns>
        /// <exception cref="ArgumentException">Thrown if the name is not consistent with the type or does not exist</exception>
        public Builder<T> With<V>(V value, string name)
        {
            Type valueT = typeof(V);
            Type objectT= typeof(T);

            var property = objectT.GetProperty(name);

            if(property == null)
            {
                throw new ArgumentException("Property not found!");
            }

            if (property.PropertyType.IsAssignableFrom(valueT))
            {
                property.SetValue(this.value, value);
            }
            else
            {
                throw new ArgumentException("Type of member is wrong!");
            }

            return this;
        }
    }
}
