using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string? FirstName { get; set; }

        [MaxLength(150)]
        public string? LastName { get; set; }

        public Guid? Organization { get; set; }

        public string? Bio { get; set; }

        public string Username { get; set; }

        public bool Deleted { get; set; }
    }

    public class UserViewModelFacade
    {
        private readonly User _user;

        public UserViewModelFacade(User user)
        {
            _user = user;
        }

        public Guid Id => _user.Id;
        
        public bool Deleted => _user.Deleted;

        public Guid? Organization => _user.Organization?.Id;
        
        public string? FirstName
        {
            get => _user.FirstName;
            set => _user.FirstName = value;
        }

        public string? LastName
        {
            get => _user.LastName;
            set => _user.LastName = value;
        }

        public string? Bio
        {
            get => _user.Bio;
            set => _user.Bio = value;
        }

        public string Username
        {
            get => _user.UserName;
            set => _user.UserName = value;
        }
    }
}
