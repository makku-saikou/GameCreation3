// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: IConnectable.cs
// Description:
// -------------------------------------------------

using GamePlay.Player;

namespace GamePlay.Item
{
    public interface IConnectable
    {
        public void Interact(PlayerController playerController);
    }
}