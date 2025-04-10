// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_10
// Description: 为了减少PFC
// -------------------------------------------------

using UnityEngine;

namespace Common.Attribute
{
    public class CommentAttribute : PropertyAttribute 
    {
        public string Comment;
        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}