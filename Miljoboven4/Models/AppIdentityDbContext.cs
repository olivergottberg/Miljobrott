﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// IdentityDbContext för inloggning

namespace Miljoboven4.Models
{
	public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
	{
		public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
	}
}
