using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CFlashcards.Models;

// TODO Add identity role for the users, so they won't be able to change cards of the other user
public class FlashcardsUser : IdentityUser
{
    // This attribute is used instead of UserName such that we dont have to write our custom SignInManager
    // because when logging in, the current one uses the Email input as UserName, such that our Email and UserName
    // have to be equal for logging in and registering to work.
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string NickName { get; set; }
}

