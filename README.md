# EditDistance

EditDistance is a simple library meant to show an implementation of different edit distance algorithms.  Currently, only Wagner-Fischer (Levenshtein edit distance) is included, though others may be added later.

## Help, Feedback, Contribute

If you have any issues or feedback, please file an issue here in Github. We'd love to have you help by contributing code for new features, optimization to the existing codebase, ideas for future releases, or fixes!

## New in v1.0.x

- Initial release
- Integrated unit test pr and fixes from @Udi-Shemer

## It's Easy

Build and run the ```Test.Levenshtein``` project.

```
C:\Code\Misc\EditDistance\src\Test.Levenshtein\bin\Debug\net7.0>Test.Levenshtein

Press ENTER to exit
String 1: Joel
String 2: joel!

Before:
   0   0 106 111 101 108  33
   0   0   1   2   3   4   5
  74   1   0   0   0   0   0
 111   2   0   0   0   0   0
 101   3   0   0   0   0   0
 108   4   0   0   0   0   0

After:
   0   0 106 111 101 108  33
   0   0   1   2   3   4   5
  74   1   1   2   3   4   5
 111   2   2   1   2   3   4
 101   3   3   2   1   2   3
 108   4   4   3   2   1   2


Edit Distance: 2
```

The first matrix shows the ASCII values of the two words in the top-most row (starting with the third column) and the left-most column (starting with the third row).

The second matrix shows the result of the calculations made.  

The bottom right cell indicates the Levenshtein edit distance.

## Version History

Refer to CHANGELOG.md for version history.
