using System;

namespace Elskom.Generic.Libs
{
    internal sealed class Tree
    {
        // Bit length codes must not exceed MAX_BL_BITS bits
        internal const int MAXBLBITS = 7;

        // end of block literal code
        internal const int ENDBLOCK = 256;

        // repeat previous bit length 3-6 times (2 bits of repeat count)
        internal const int REP36 = 16;

        // repeat a zero length 3-10 times  (3 bits of repeat count)
        internal const int REPZ310 = 17;

        // repeat a zero length 11-138 times  (7 bits of repeat count)
        internal const int REPZ11138 = 18;

        // The lengths of the bit length codes are sent in order of decreasing
        // probability, to avoid transmitting the lengths for unused bit
        // length codes.
        internal const int BufSize = 8 * 2;

        // see definition of array dist_code below
        internal const int DISTCODELEN = 512;

        // extra bits for each length code
        internal static ReadOnlySpan<int> ExtraLbits => new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0,
        };

        // extra bits for each distance code
        internal static ReadOnlySpan<int> ExtraDbits => new[]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7,
            8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13,
        };

        // extra bits for each bit length code
        internal static ReadOnlySpan<int> ExtraBlbits => new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7,
        };

        internal static ReadOnlySpan<int> BaseLength => new[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 20, 24, 28, 32, 40, 48, 56,
            64, 80, 96, 112, 128, 160, 192, 224, 0,
        };

        internal static ReadOnlySpan<int> BaseDist => new[]
        {
            0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384,
            512, 768, 1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384,
            24576,
        };

        internal static ReadOnlySpan<byte> BlOrder => new byte[]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15,
        };

        internal static ReadOnlySpan<byte> DistCode => new byte[]
        {
            0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8,
            9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
            10, 10, 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0,  0,  16, 17,
            18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22,
            22, 22, 23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
        };

        internal static ReadOnlySpan<byte> LengthCode => new byte[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 12, 12,
            13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 16,
            16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18,
            18, 18, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20, 20, 20, 20, 20,
            20, 20, 20, 20, 20, 20, 20, 20, 20, 21, 21, 21, 21, 21, 21, 21, 21,
            21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 22, 22, 22,
            22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28,
        };

        private const int MAXBITS = 15;

        // private const int BLCODES = 19;
        // private const int DCODES = 30;
        private const int LITERALS = 256;
        private const int LENGTHCODES = 29;
        private const int LCODES = LITERALS + 1 + LENGTHCODES;
        private const int HEAPSIZE = (2 * LCODES) + 1;

        internal short[] DynTree { get; set; } // the dynamic tree

        internal int MaxCode { get; private set; } // largest code with non zero frequency

        internal StaticTree StatDesc { get; set; } // the corresponding static tree

        // Mapping from a distance to a distance code. dist is the distance - 1 and
        // must not have side effects. _dist_code[256] and _dist_code[257] are never
        // used.
        internal static int D_code(int dist) => dist < 256 ? DistCode[dist] : DistCode[256 + SupportClass.URShift(dist, 7)];

        // Generate the codes for a given tree and bit counts (which need not be
        // optimal).
        // IN assertion: the array bl_count contains the bit length statistics for
        // the given tree and the field len is set for all tree elements.
        // OUT assertion: the field code is set for all tree elements of non
        //     zero code length.
        internal static void Gen_codes(short[] tree, int max_code, short[] bl_count)
        {
            var next_code = new short[MAXBITS + 1]; // next code value for each bit length
            short code = 0; // running code value
            int bits; // bit index
            int n; // code index

            // The distribution counts are first used to generate the code values
            // without bit reversal.
            for (bits = 1; bits <= MAXBITS; bits++)
            {
                next_code[bits] = code = (short)((code + bl_count[bits - 1]) << 1);
            }

            // Check that the bit counts in bl_count are consistent. The last code
            // must be all ones.
            // Assert (code + bl_count[MAX_BITS]-1 == (1<<MAX_BITS)-1,
            //        "inconsistent bit counts");
            // Tracev((stderr,"\ngen_codes: max_code %d ", max_code));
            for (n = 0; n <= max_code; n++)
            {
                int len = tree[(n * 2) + 1];
                if (len == 0)
                {
                    continue;
                }

                // Now reverse the bits
                tree[n * 2] = (short)Bi_reverse(next_code[len]++, len);
            }
        }

        // Reverse the first len bits of a code, using straightforward code (a faster
        // method would use a table)
        // IN assertion: 1 <= len <= 15
        internal static int Bi_reverse(int code, int len)
        {
            var res = 0;
            do
            {
                res |= code & 1;
                code = SupportClass.URShift(code, 1);
                res <<= 1;
            }
            while (--len > 0);
            return SupportClass.URShift(res, 1);
        }

        // Compute the optimal bit lengths for a tree and update the total bit length
        // for the current block.
        // IN assertion: the fields freq and dad are set, heap[heap_max] and
        //    above are the tree nodes sorted by increasing frequency.
        // OUT assertions: the field len is set to the optimal bit length, the
        //     array bl_count contains the frequencies for each bit length.
        //     The length opt_len is updated; static_len is also updated if stree is
        //     not null.
        internal void Gen_bitlen(Deflate s)
        {
            var tree = this.DynTree;
            ReadOnlySpan<short> stree;
            switch (this.StatDesc.StaticTreeOption)
            {
                case 0:
                    stree = StaticTree.StaticLtree;
                    break;

                case 1:
                    stree = StaticTree.StaticDtree;
                    break;

                case 2:
                default:
                    stree = null;
                    break;
            };
            ReadOnlySpan<int> extra;
            switch (this.StatDesc.ExtraBitOption)
            {
                case 0:
                    extra = ExtraLbits;
                    break;

                case 1:
                    extra = ExtraDbits;
                    break;

                case 2:
                default:
                    extra = ExtraBlbits;
                    break;
            };
            var base_Renamed = this.StatDesc.ExtraBase;
            var max_length = this.StatDesc.MaxLength;
            int h; // heap index
            int n, m; // iterate over the tree elements
            int bits; // bit length
            int xbits; // extra bits
            short f; // frequency
            var overflow = 0; // number of elements with bit length too large

            for (bits = 0; bits <= MAXBITS; bits++)
            {
                s.BlCount[bits] = 0;
            }

            // In a first pass, compute the optimal bit lengths (which may
            // overflow in the case of the bit length tree).
            tree[(s.Heap[s.HeapMax] * 2) + 1] = 0; // root of the heap

            for (h = s.HeapMax + 1; h < HEAPSIZE; h++)
            {
                n = s.Heap[h];
                bits = tree[(tree[(n * 2) + 1] * 2) + 1] + 1;
                if (bits > max_length)
                {
                    bits = max_length;
                    overflow++;
                }

                tree[(n * 2) + 1] = (short)bits;

                // We overwrite tree[n*2+1] which is no longer needed
                if (n > this.MaxCode)
                {
                    continue; // not a leaf node
                }

                s.BlCount[bits]++;
                xbits = 0;
                if (n >= base_Renamed)
                {
                    xbits = extra[n - base_Renamed];
                }

                f = tree[n * 2];
                s.OptLen += f * (bits + xbits);
                if (stree != null)
                {
                    s.StaticLen += f * (stree[(n * 2) + 1] + xbits);
                }
            }

            if (overflow == 0)
            {
                return;
            }

            // This happens for example on obj2 and pic of the Calgary corpus
            // Find the first bit length which could increase:
            do
            {
                bits = max_length - 1;
                while (s.BlCount[bits] == 0)
                {
                    bits--;
                }

                s.BlCount[bits]--; // move one leaf down the tree
                s.BlCount[bits + 1] = (short)(s.BlCount[bits + 1] + 2); // move one overflow item as its brother
                s.BlCount[max_length]--;

                // The brother of the overflow item also moves one step up,
                // but this does not affect bl_count[max_length]
                overflow -= 2;
            }
            while (overflow > 0);

            for (bits = max_length; bits != 0; bits--)
            {
                n = s.BlCount[bits];
                while (n != 0)
                {
                    m = s.Heap[--h];
                    if (m > this.MaxCode)
                    {
                        continue;
                    }

                    if (tree[(m * 2) + 1] != bits)
                    {
                        s.OptLen = (int)(s.OptLen + ((bits - (long)tree[(m * 2) + 1]) * tree[m * 2]));
                        tree[(m * 2) + 1] = (short)bits;
                    }

                    n--;
                }
            }
        }

        // Construct one Huffman tree and assigns the code bit strings and lengths.
        // Update the total bit length for the current block.
        // IN assertion: the field freq is set for all tree elements.
        // OUT assertions: the fields len and code are set to the optimal bit length
        //     and corresponding code. The length opt_len is updated; static_len is
        //     also updated if stree is not null. The field max_code is set.
        internal void Build_tree(Deflate s)
        {
            var tree = this.DynTree;
            ReadOnlySpan<short> stree;
            switch (this.StatDesc.StaticTreeOption)
            {
                case 0:
                    stree = StaticTree.StaticLtree;
                    break;

                case 1:
                    stree = StaticTree.StaticDtree;
                    break;

                case 2:
                default:
                    stree = null;
                    break;
            };
            var elems = this.StatDesc.Elems;
            int n, m; // iterate over heap elements
            var max_code = -1; // largest code with non zero frequency
            int node; // new node being created

            // Construct the initial heap, with least frequent element in
            // heap[1]. The sons of heap[n] are heap[2*n] and heap[2*n+1].
            // heap[0] is not used.
            s.HeapLen = 0;
            s.HeapMax = HEAPSIZE;

            for (n = 0; n < elems; n++)
            {
                if (tree[n * 2] != 0)
                {
                    s.Heap[++s.HeapLen] = max_code = n;
                    s.Depth[n] = 0;
                }
                else
                {
                    tree[(n * 2) + 1] = 0;
                }
            }

            // The pkzip format requires that at least one distance code exists,
            // and that at least one bit should be sent even if there is only one
            // possible code. So to avoid special checks later on we force at least
            // two codes of non zero frequency.
            while (s.HeapLen < 2)
            {
                node = s.Heap[++s.HeapLen] = max_code < 2 ? ++max_code : 0;
                tree[node * 2] = 1;
                s.Depth[node] = 0;
                s.OptLen--;
                if (stree != null)
                {
                    s.StaticLen -= stree[(node * 2) + 1];
                }

                // node is 0 or 1 so it does not have extra bits
            }

            this.MaxCode = max_code;

            // The elements heap[heap_len/2+1 .. heap_len] are leaves of the tree,
            // establish sub-heaps of increasing lengths:
            for (n = s.HeapLen / 2; n >= 1; n--)
            {
                s.Pqdownheap(tree, n);
            }

            // Construct the Huffman tree by repeatedly combining the least two
            // frequent nodes.
            node = elems; // next internal node of the tree
            do
            {
                // n = node of least frequency
                n = s.Heap[1];
                s.Heap[1] = s.Heap[s.HeapLen--];
                s.Pqdownheap(tree, 1);
                m = s.Heap[1]; // m = node of next least frequency

                s.Heap[--s.HeapMax] = n; // keep the nodes sorted by frequency
                s.Heap[--s.HeapMax] = m;

                // Create a new node father of n and m
                tree[node * 2] = (short)(tree[n * 2] + tree[m * 2]);
                s.Depth[node] = (byte)(System.Math.Max(s.Depth[n], s.Depth[m]) + 1);
                tree[(n * 2) + 1] = tree[(m * 2) + 1] = (short)node;

                // and insert the new node in the heap
                s.Heap[1] = node++;
                s.Pqdownheap(tree, 1);
            }
            while (s.HeapLen >= 2);

            s.Heap[--s.HeapMax] = s.Heap[1];

            // At this point, the fields freq and dad are set. We can now
            // generate the bit lengths.
            this.Gen_bitlen(s);

            // The field len is now set, we can generate the bit codes
            Gen_codes(tree, max_code, s.BlCount);
        }
    }
}