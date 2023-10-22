using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CFlashcards.Models;

public class FlashcardsUser : IdentityUser
    // This class extends the IdentityUser. Here we add additional attributes to the user class of our application.
{
    // This attribute is used instead of UserName such that we dont have to write our custom SignInManager
    // because when logging in, the current one uses the Email input as UserName, such that our Email and UserName
    // have to be equal for logging in and registering to work.
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string NickName { get; set; }
}

