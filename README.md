# G12 ChessApplication

Chess game implemented in C# & .NET as a project for 62413 Advanced Object Oriented Programming.

## Code Contribution
To maintain a proper structure it is advised to follow the style guide for C# by Microsoft ([Link](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions))
This ensures that classes, functions, variables names and other are consitent between contributors.

> Note: ChatGPT or other tools can always be used last to ensure the style guide is adhered to.

### Branching
When creating a new feature, create a new branch based on the development branch. 
This ensures that the development branch only gets populated with working tested code.

#### Branch naming
| Prefix | Description |
|--------|-------------|
| `feature/` | Used for new features |
| `doc/` | Used for documentation changes |
| `refactor/` | Used for refactoring code |
| `style/` | Used for code style changes |
| `bugfix/` | Used for bug fixes |
| `hotfix/` | Used for critical bug fixes |

Also add a surfix with your initials to make it easier to see who is working on the branch.

| Suffix | Description |
|--------|-------------|
| `/<initials>` | Your initials |
| `/<team>` | The team you are working with |

### Commit messeages
When making commits use the following format for the commit message:
```plaintext
<type>: <short desciption>
(optional) <long description> 
```

| Type | Description |
|------|-------------|
| `feat:` | A new feature. (Correlates with MINOR in Semantic Versioning) |
| `fix:` | A bug fix. (Correlates with PATCH in Semantic Versioning)|
| `docs:` | Documentation changes |
| `style:` | Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc) |
| `refactor:` | A code change that neither fixes a bug nor adds a feature |

Example:
```plaintext
feat: Add new feature to the code

This commit adds a new feature to the code. 
```

### Pull Requests
When a feature has been implemented and tested locally. A pull request can be made on GitHub onto the development branch
The implementation will be reviewed by a team member and accepted or denied, with comments on potential changes.
	
