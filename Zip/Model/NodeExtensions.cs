using System.Collections.Generic;
using MoreLinq.Extensions;

namespace Model
{
    public static class NodeExtensions
    {
        public static Node TakeNodeByMinCount(this List<Node> nodes)
        {
            var minCountNode = nodes.MinBy(x => x.Count).FirstOrDefault();
            nodes.Remove(minCountNode);
            return minCountNode;
        }
    }
}