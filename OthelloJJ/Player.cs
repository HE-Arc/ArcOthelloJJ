using System.Windows.Media;

namespace OthelloJJ
{
    class Player
    {
        private readonly ImageSource image;
        public ImageSource Image
        {
            get { return image; }
        }
        private readonly int val;
        public int Val
        {
            get { return val; }
        }

        public Player(ImageSource image, int value)
        {
            this.image = image;
            this.val = value;
        }
    }
}
