using System;
using System.Windows.Media;

namespace OthelloJJ
{
    class Player
    {
        private readonly ImageSource image;
        private TimeSpan time { get; set; }
        public ImageSource Image
        {
            get { return image; }
        }
        private readonly int val;
        public int Val
        {
            get { return val; }
        }

        public Player(ImageSource image, int value,TimeSpan time)
        {
            this.image = image;
            this.val = value;
            this.time = time;
        }
    }
}
