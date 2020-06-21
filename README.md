AlFinR.Identity.Dapper
============

This is not a plug-and-play library, but give you the complete control over Identity models that direct refer to Microsoft.Extensions.Identity.Core package.

This project best fit for replacement of Microsoft.Extensions.Identity.Stores package, which provide storage method based on Entity Framework Core.

## Detail

I created this project for anyone like me, would love to use a well defined identity procedure from Microft themself, but require better performance tham EF Core(which is Dapper in this specific case).

Therefore, i will assume you guys got enough ideas of how Database work and how importance treat it nicely.

Fell free to cutoff, add or modify any properties that you need, change the column type, add some reasonable indexes,... We are completely free from code-first.

Just be careful to navigation entity like IdentityUserRole, i don't see any resons to add or remove or modify anything from navigation entity, so i hard-coded them, manually modify the repository if you ever need it to change.

Key points solution on github:
- Direct dependencies on Microsoft.Extensions.Identity.Core package (not hand-define the whole things).
- Direct dependencies on Dapper and Dapper.Contrib (not self-define query builder).
- Compile to dll or just add project to use(Target .Net Standard 2.0).
- Dont have nuget package (of-course, i'm not intend to build it plug-and-play).
- Originally intended to working with .Net Core, so i use Repository parttern.

One last note, I know DapperExtensions is a solution for composite PK, but this project target .Net Standard 2.0, and can't restore DapperExtensions package natively(warning restore by .Net Framework 4.7x), so it could cause compatible issues. If anyone have any idea to solve this please let me know, thanks you guys (XD).

## Knowledge base

Required to work flawlessly:
Concept of how Microsoft.Extensions.Identity.Store work.

Nice to have:
- Dapper
- Dapper.Contrib
- DapperExtensions

## Dependencies

- Microsoft.Extensions.Identity.Store(3.1.5)
- Microsoft.Extensions.Identity.Core(3.1.5)
- Dapper(2.0.35)
- Dapper.Contrib(2.0.35)

## Tutorial

Please take a look at Tutorial text file for detail.