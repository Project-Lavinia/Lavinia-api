using System.ComponentModel.DataAnnotations.Schema;

namespace Lavinia.Api.Models
{
    /// <summary>
    /// As EF does not support lists of native types, this class can be used to store values in a list
    /// The class can hold just values, keys or a combination of both depending on need.
    /// It can after retrieval from storage be converted to a Dictionary or List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListElement<T>
    {
        /// <summary>
        /// ID only for use by the Database
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; } = default!;

        /// <summary>
        /// For storing an identifier for the value
        /// </summary>
        public string Key { get; init; } = default!;

        /// <summary>
        /// For storing the actual value
        /// </summary>
        public T Value { get; init; } = default!;
    }
}