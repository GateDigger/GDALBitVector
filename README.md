# GDALBitVector
is GateDigger's implementation of arbitrary length bitvector.

## Functionality

### Mutable (mutating) operations
- Single-bit write
- 32-bit block write
- Bitwise not, and, or, xor

### Immutable operations
- 32-bit block write
- Bitwise not, and, or, xor, shifts
- Concatenation
- Resize

### Other
- ==, !=, Equals, ...
- bool[] and UInt32[] constructors
- Parse, ToString
- Cloning

## Compilation dependencies
- Fody (6.8.0)
- InlineIL.Fody (1.7.4)

## License

MIT License

Copyright (c) 2023 GateDigger

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
