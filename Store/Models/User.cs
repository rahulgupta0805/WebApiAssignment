using System.ComponentModel.DataAnnotations;

namespace Assignment.Service.Store.Models
{
    public class User
    {
        public User(int id, string firstName, string lastName, string emailId, 
            DateTime createdDate, DateTime updatedDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            EmailId = emailId;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
        }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
