using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CFlashcards.Areas.Identity.Data;

// Add profile data for application users by adding properties to the FlashcardsUser class
public class FlashcardsUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName="nvarchar(100)")]  //atribute collumn assigned with datatype TypeName
    public string FirstName { get; set; }
    //next property
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]  //atribute collumn assigned with datatype TypeName
    public string LastName { get; set; }
}

