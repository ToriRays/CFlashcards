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
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }
    //next property
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }
    [PersonalData]
    //navigation property
    public virtual List<Deck>? Decks { get; set; }
}

