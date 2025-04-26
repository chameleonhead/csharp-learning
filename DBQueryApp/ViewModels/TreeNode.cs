using System.Collections.Generic;

namespace DBQueryApp.ViewModels;

public class TreeNode
{
    public string Name { get; set; } = string.Empty;
    public List<TreeNode> Children { get; set; } = [];
}
