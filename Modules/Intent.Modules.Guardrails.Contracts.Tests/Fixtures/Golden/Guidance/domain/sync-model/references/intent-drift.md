---
contentHash: 8F8B45029DA3B30A15838565FAB6FB7CCDBC14F035C77FF41D83AF1289A6308D
---
# Resolving intent drift

An intent row (*raises*) is about one association the model gives an element: a `Domain Event Origin`, from a constructor or operation to a Domain Event it raises. It says the recipe found no sign of it in the element's code: `code (none)`, with what it looked for in brackets, or that the fact can't be verified.

One such row can mean three things, and each has a different fix:

- the element still does it, in a form the recipe doesn't read;
- another element does it now;
- nothing does it any more.

Work out which before you change the model or the code. Never delete an association until you know where the work went: deleting one that is still true leaves the model wrong, and nothing reports that.

## 1. Find where the work is now

Open the element's code at the row's location. Then search the code for the name after `model` (the event, entity, operation, command or query), leaving out its own declaration and the tests. What you find decides the fix:

| You find | What to do |
| --- | --- |
| The element still does it, in code the recipe doesn't read: another bus or repository API, or a private method, helper or factory it calls | Keep the association. The recipe misreads the code: fix it with **configure-drift-check** (its technology guide's "Adapting to house style"), and change neither side |
| Another constructor, operation or handler does it now | The work moved. Delete the association here, and add the same association from the element that does it now, modelling that element first if the model doesn't have it. The next run checks it there, which confirms the move |
| Nothing mentions the name, and the Software Factory proposes adding its type back | The code renamed or removed the target. If it was renamed, rename its element in the model (don't delete it and create another): the association stays. If it was removed, as the next row says |
| Nothing does it any more | The code stopped doing it. If that was deliberate (a change made in this piece of work, or one the developer asked for), delete the association. Otherwise ask the developer whether to delete it or put the code back, in the same `ask_user_question` as the other contradictions |

## 2. Many rows at once

Many intent rows of one kind after one change, all `code (none)` or all can't be verified, mean the recipe no longer reads the code: a new bus API, handlers moved to another project, a renamed handler interface. Fix the recipe once (its `files`, `find` or read cases) with **configure-drift-check**, check what it reads with **validate-drift-recipes**, and leave the model as it is.

## 3. Can't be verified

An intent fact can't be verified when the recipe didn't find the element's code. If the code moved (another project, file or class name), fix the recipe's `files` or `find`. If the code no longer has the element, its work moved or was dropped: resolve each of its rows as in §1.

## 4. What is never reported

The check compares the code with the model's associations, so it never reports something the code does that the model doesn't show, such as a domain event a constructor or operation starts raising. When the work moved, add the association where it went (§1), and whenever you see code doing something the model doesn't show, model it.
