using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;


namespace WindowsFormsApplication1ProjectF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            panel2.Visible = false;
        }

        Bitmap bm;//this is going to save the image and every edit i apply to it
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)//Info menu button
        {
            MessageBox.Show("This is an image viewer application" + "\n" + "There are some interesting features!", "Information");
        }

        string path;//this will save the path of the image i chose
        Bitmap imgOriginal;//this will save the original image without any edits
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)//load image menu button
        {
            trackBar1.Value = 0;//the zoom is at zero in order not to mess the imgOriginal bitmap image
            //i open a open file box to choose only one file with specific extensions (must be image) 
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG" })
            {
                if(ofd.ShowDialog() == DialogResult.OK )//if we chose a valid file
                {
                    path = ofd.FileName;//we save the file's whole path name to our path variable
                    name = Path.GetFileNameWithoutExtension(path);//we get the name of the file without extensions
                    pictureBox1.Image = Image.FromFile(ofd.FileName);//we load the image to the picturebox
                    imgOriginal = (Bitmap)Bitmap.FromFile(path);//we load the original image to out bitmap variable
                    pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);//we send the picturebox at the center
                    pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);//it sometimes requires to send it to center twice for big images
                    bm = (Bitmap)Bitmap.FromFile(path);
                }
            }
        }

        Image Zoom(Image img, Size size)//class that resizes the image
        {//i create new bitmap that takes as parameters the original image and the new width + new height that i calculate considering the picturebox's width and height
            Bitmap bmp = new Bitmap(img, pictureBox1.Width + (pictureBox1.Width * size.Width / 100), pictureBox1.Height + (pictureBox1.Height * size.Height / 100));
            Graphics g = Graphics.FromImage(bmp);//it creates a new graphic fro out bmp bitmap
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;//it creates a 2d graphic interjection using our previous graphic
            return bmp;
        }

        private void stretchToolStripMenuItem_Click(object sender, EventArgs e)//stretch image menu button
        {
            pictureBox1.Height = 400;
            pictureBox1.Width = 550;
            pictureBox1.Image = bm;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //for large images it needs to do the stretch procedure twice to have the visual effect we want
            pictureBox1.Height = 400;
            pictureBox1.Width = 550;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void fullSizeToolStripMenuItem_Click(object sender, EventArgs e)//full image size menu button
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = bm;
            pictureBox1.Location= new Point((Width / 2)- pictureBox1.Width/2, (Height / 2) - pictureBox1.Height/2);//force it to center
        }        

        private void trackBar1_Scroll(object sender, EventArgs e)//zoom in and zoom out trackbar
        {
            if(trackBar1.Value > 0 && pictureBox1.Image != null)//at value zero image is at normal mode
            {
                pictureBox1.Height = bm.Height;//in order to have better view of the zoom in and zoom out effect we set the pictureBox at full size
                pictureBox1.Width = bm.Width;
                pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;//keeping the image at the center
                pictureBox1.Image = Zoom(bm, new Size(trackBar1.Value, trackBar1.Value));
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)//remove image button (changed name)
        {
            if (pictureBox1.Image != null)
            {//it removes the image in order to have an empty form
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                pictureBox1.InitialImage = null;
                pictureBox1.Height = 400;
                pictureBox1.Width = 550;
                panel2.Visible = true;
                richTextBox1.Clear();
                panel2.Visible = false;
                pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);
                bm = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//at close we dispose the image
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
        }


        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)//grayscale
        {
            if (pictureBox1.Image != null)
            {
                GrayScaleImage(bm);//if there's is an image we call the grayscale method
            }
        }

        public void  GrayScaleImage(Bitmap original)//the grayscale method
        {
            //it creates a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //it gets a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //creates the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
            });

            //creates some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //sets the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draws the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //it disposes the Graphics object
            g.Dispose();
            pictureBox1.Image = newBitmap;
            bm = newBitmap;
        }

        private void negativeColoursToolStripMenuItem_Click(object sender, EventArgs e)//negative colors
        {
            if (pictureBox1.Image != null)
            {//if there's is an image i call the negative colors method
                InvertImageColorMatrix(bm);
            }

        }

        public void  InvertImageColorMatrix(Bitmap originalImg)//negative colors method
        {
            Bitmap invertedBmp = new Bitmap(originalImg.Width, originalImg.Height);

            //Setup color matrix
            ColorMatrix clrMatrix = new ColorMatrix(new float[][]
                                                    {
                                                    new float[] {-1, 0, 0, 0, 0},
                                                    new float[] {0, -1, 0, 0, 0},
                                                    new float[] {0, 0, -1, 0, 0},
                                                    new float[] {0, 0, 0, 1, 0},
                                                    new float[] {1, 1, 1, 0, 1}
                                                    });

            using (ImageAttributes attr = new ImageAttributes())
            {
                //it attaches the matrix to image attributes
                attr.SetColorMatrix(clrMatrix);

                using (Graphics g = Graphics.FromImage(invertedBmp))
                {//using the original image it draws another image with the specified image attributes
                    g.DrawImage(originalImg, new Rectangle(0, 0, originalImg.Width, originalImg.Height),
                                0, 0, originalImg.Width, originalImg.Height, GraphicsUnit.Pixel, attr);
                }
            }

            pictureBox1.Image = invertedBmp;
            bm = invertedBmp;
        }

       

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)//reload image
        {
            pictureBox1.Image = imgOriginal;//we use the imgOriginal that hasn't changed by any edits
            bm = imgOriginal;
            pictureBox1.Height = 400;
            pictureBox1.Width = 550;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Height = 400;//we do this twice because for some large images it doesn't work at once
            pictureBox1.Width = 550;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        }


        private void pictureBox5_Click(object sender, EventArgs e)//turn left button
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                bm.RotateFlip(RotateFlipType.Rotate270FlipNone);//it rotates
                pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);//it goes in the middle

                pictureBox1.Image = bm;              
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)//turn right button
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);

                pictureBox1.Image = bm;
            }
        }


        string info,name;//variables that will save the image's name anf the image's info
        private void addInfoToolStripMenuItem_Click(object sender, EventArgs e)//add info button
        {
            if(pictureBox1.Image != null)
            {//it changes the image's name and info
                menuStrip1.Enabled = false;
                trackBar1.Enabled = false;
                pictureBox2.Enabled = false;
                pictureBox5.Enabled = false;
                pictureBox6.Enabled = false;
                panel2.Visible = true;
                textBox1.Text = name;
                richTextBox1.Text = info;
            }
        }

        private void printInfoToolStripMenuItem_Click(object sender, EventArgs e)//print info button
        {
            if (pictureBox1.Image != null)
            {
                MessageBox.Show(info,name);//it shows the image's ingo
            }
        }

        private void button1_Click(object sender, EventArgs e)//cancel button at the image info 
        {//it won't save the changes
            menuStrip1.Enabled = true;
            trackBar1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox5.Enabled = true;
            pictureBox6.Enabled = true;
            panel2.Visible = false;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)//save as menu button
        {
            string savepath, text;
            if (bm != null)
            {
                using (var fbd = new FolderBrowserDialog())//we open a folder selection window
                {
                    DialogResult result = fbd.ShowDialog();

                    savepath = fbd.SelectedPath;
                }
                text = richTextBox1.Text;
                bm.Save(System.IO.Path.Combine(savepath, name), ImageFormat.Png);//we save the image at the specified folder with the specified name
                File.WriteAllText(savepath + "/@" + name,
                  text);
                //at the same folder we create a .txt with the same name as the image's +'@' in front with the image's info
            }
        }

        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)//a class that returns a string array of the paths of files i want to show in a sllideshow
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;//it will search the folder's files only and not any files in sub folders
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));//it gets the name of the files with the specified filter and it puts them into the string array list
            }
            
            
            return filesFound.ToArray();
        }

        public int imgcounter,imgnumber;//first variable will save the number of the image that we show out of the total number of images that the 2nd variable saves
        public string[] files;
        private void pictureBox2_Click(object sender, EventArgs e)//slideshow button
        {
            imgnumber = 0;
            string folderpath;
            using (var fbd = new FolderBrowserDialog())//it opens a dialog window to pick a folder
            {
                DialogResult result = fbd.ShowDialog();

                folderpath = fbd.SelectedPath;
            }
            if (folderpath != "")//only if i have picked something
            {
                String searchFolder = folderpath;
                var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                files = GetFilesFrom(searchFolder, filters, false);//we call the class


                foreach (string pimg in files)
                {//we calculate the number of the images that are inside the folder
                    //Console.WriteLine(pimg);
                    imgnumber++;
                }

                //Console.Read();
                imgcounter = 0;
                try//it will try to dispose any images we have at the pictureBox
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    pictureBox1.InitialImage = null;
                }
                catch { }
                trackBar1.Enabled = false;//we disable some edit options
                pictureBox2.Enabled = false;
                SlideShower();//we call the slideshower method
            }
        }

        public void SlideShower()//a method that with the help of a counter will execute the slideshow
        {
            if (imgcounter < imgnumber)//repeat as many times as there are images selected
            {
                pictureBox1.Load(files[imgcounter]);//it loads the image
                bm = (Bitmap)Bitmap.FromFile(files[imgcounter]);//we create a bitmap image if we want to do some edits while the slideshow
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;//stretch images
                pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);//they must be in the center
                timer1.Start();//the timers starts ehich has interval 2 seconds, the time that each image will be shown
            }
            else
            {//if we are finished
                trackBar1.Enabled = true;
                pictureBox2.Enabled = true;
            }
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)//zoom menu button
        {
            pictureBox1.Height = 400;
            pictureBox1.Width = 550;
            pictureBox1.Image = bm;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //for big images it needs to do the stretch procedure twice to have the visual effect we want
            pictureBox1.Height = 400;
            pictureBox1.Width = 550;
            pictureBox1.Location = new Point((Width / 2) - pictureBox1.Width / 2, (Height / 2) - pictureBox1.Height / 2);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }


        private void timer1_Tick(object sender, EventArgs e)//the timer that helps the slideshow to execute
        {
            timer1.Stop();//top the timer
            imgcounter++;//next image
            pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
            pictureBox1.InitialImage = null;
            SlideShower();//call the method again for next image
        }

        private void button2_Click(object sender, EventArgs e)//save button at the image info
        {
            menuStrip1.Enabled = true;
            trackBar1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox5.Enabled = true;
            pictureBox6.Enabled = true;
            name = textBox1.Text;
            info = richTextBox1.Text;
            panel2.Visible = false;
        }
    }
}
