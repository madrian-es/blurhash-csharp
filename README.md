# blurhash-csharp
Blurhash is an algorithm that lets you transform image data into a small text representation of a blurred version of the image. This is useful since this small textual representation can be included when sending objects that may have images attached around, which then can be used to quickly create a placeholder for images that are still loading or that should be hidden behind a content warning.

This is a .NET console application that closely follows the original implementation by Dag Ågren. It should give the same results as the C version of his implementation. Magick.NET is required to load an image. You could also convert a bitmap to an RGBA byte array if you don't want to use Magick.

Currently, only the encoding functionality has been ported, decoding will be available soon.

If you check out the [original repo](https://github.com/woltapp/blurhash) you can find official implementations in C, Swift, TypeScript and Kotlin.
