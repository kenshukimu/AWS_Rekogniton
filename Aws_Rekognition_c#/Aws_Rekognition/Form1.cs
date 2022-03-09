using AForge.Video.DirectShow;
using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Aws_Rekognition
{
    public partial class Form1 : Form
    {
        AmazonRekognitionClient rekognitionClient;

        VideoCaptureDevice videoCaptureDevice = new VideoCaptureDevice();
        FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        public System.Drawing.Image ReceivedData;
        public Form1()
        {
            InitializeComponent();
            rekognitionClient = new AmazonRekognitionClient(Properties.Resources.accessKey,
                                                            Properties.Resources.secretKey, 
                                                            RegionEndpoint.APNortheast2);
            
        }
               
        private void getAWSinfo(String fileName, String kb)
        {
            richTextBox1.Text = "";

            String rtn = String.Empty;

            String photo = fileName;

            byte[] data;

            if(fileName != null)
            {
                data = File.ReadAllBytes(fileName);
            }
            else
            {
                if(kb.Equals("1"))
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (Bitmap tempImage = new Bitmap(pictureBox01.Image))
                    {
                        tempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //error here System.ArgumentNullException 
                        data=  ms.ToArray();
                    }
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (Bitmap tempImage = new Bitmap(pictureBox02.Image))
                    {
                        tempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //error here System.ArgumentNullException 
                        data = ms.ToArray();
                    }
                }
            }

            DetectFacesRequest detectFacesRequest = new DetectFacesRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = new MemoryStream(data),
                },
                Attributes = new List<string>() { "ALL" }
            };           

            try
            {
                DetectFacesResponse detectFacesResponse = rekognitionClient.DetectFaces(detectFacesRequest);
                bool hasAll = detectFacesRequest.Attributes.Contains("ALL");
                foreach(FaceDetail face in detectFacesResponse.FaceDetails)
                {
                    rtn += string.Format("사진 크기: top={0} left={1} width={2} height={3}", face.BoundingBox.Left,
                        face.BoundingBox.Top, face.BoundingBox.Width, face.BoundingBox.Height);

                    rtn += "\r\n";

                    rtn += string.Format("사람 정확도: {0}\nLandmarks: {1}\nPose: pitch={2} roll={3} yaw={4}\nQuality: {5}",
                                            face.Confidence, face.Landmarks.Count, face.Pose.Pitch,
                                            face.Pose.Roll, face.Pose.Yaw, face.Quality);
                    rtn += "\r\n";

                    if (hasAll)
                    {
                        rtn += string.Format("사진속 인물의 추정나이는 " +
                            face.AgeRange.Low + "살 에서 " + face.AgeRange.High + "살 사이입니다.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            rtn += "\r\n";
            rtn += "=========================================================";
            rtn += "\r\n";


            richTextBox1.Text = rtn;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            if (
                (label1.Text.Trim() == "" && this.pictureBox01.Image == null)
                     || (label2.Text.Trim() == "" && this.pictureBox02.Image == null)
               ) 
            { 
                richTextBox1.Text = "사진이 두장 필요합니다.";
                return;
            }

            float similarityThreshold = 70F;

            Amazon.Rekognition.Model.Image imageSource = new Amazon.Rekognition.Model.Image();
            try
            {
                if(this.label1.Text != null && this.label1.Text.Trim() != "")
                {
                    FileStream fs = new FileStream(this.label1.Text, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageSource.Bytes = new MemoryStream(data);
                }
                else
                {   
                    using (MemoryStream ms = new MemoryStream())
                    using (Bitmap tempImage = new Bitmap(pictureBox01.Image))
                    {
                        tempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //error here System.ArgumentNullException 
                        imageSource.Bytes = ms;
                    }                    
                }               
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to load source image: {this.label1.Text}");
                return;
            }

            Amazon.Rekognition.Model.Image imageTarget = new Amazon.Rekognition.Model.Image();

            try
            {
                if (this.label2.Text != null && this.label2.Text.Trim() != "")
                {
                    FileStream fs = new FileStream(this.label2.Text, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageTarget.Bytes = new MemoryStream(data);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (Bitmap tempImage = new Bitmap(pictureBox02.Image))
                    {
                        tempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //error here System.ArgumentNullException 
                        imageTarget.Bytes = ms;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load target image: {this.label2.Text}");
                Console.WriteLine(ex.Message);
                return;
            }

            var compareFacesRequest = new CompareFacesRequest
            {
                SourceImage = imageSource,
                TargetImage = imageTarget,
                SimilarityThreshold = similarityThreshold,
            };

            var compareFacesResponse = rekognitionClient.CompareFaces(compareFacesRequest);

            compareFacesResponse.FaceMatches.ForEach(match =>
            {
                ComparedFace face = match.Face;
                BoundingBox position = face.BoundingBox;
                //richTextBox1.Text += String.Format($"사진 인물은 {position.Left} {position.Top} matches with {match.Similarity}% 정확합니다..");
                richTextBox1.Text += String.Format($"사진 인물 중에 {match.Similarity}% 동일인물일 가능성이 있습니다.");
                richTextBox1.Text += "\r\n";

            });

            Console.WriteLine($"Found {compareFacesResponse.UnmatchedFaces.Count} face(s) that did not match.");

            richTextBox1.Text += $"Found {compareFacesResponse.UnmatchedFaces.Count} face(s) that did not match.";

        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            pictureBox01.SizeMode = PictureBoxSizeMode.StretchImage;
            OpenFileDialog op = new OpenFileDialog();

            if (op.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox01.Controls.Clear();

                this.pictureBox01.ImageLocation = op.FileName;

                this.label1.Text = op.FileName;

                getAWSinfo(op.FileName, "");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox02.SizeMode = PictureBoxSizeMode.StretchImage;
            OpenFileDialog op = new OpenFileDialog();

            if (op.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox02.Controls.Clear();

                this.pictureBox02.ImageLocation = op.FileName;

                this.label2.Text = op.FileName;

                getAWSinfo(op.FileName, "");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] data = null;

            if (this.label1.Text != null && this.label1.Text.Trim() != "")
            {
                data = File.ReadAllBytes(this.label1.Text);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                using (Bitmap tempImage = new Bitmap(pictureBox01.Image))
                {
                    tempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //error here System.ArgumentNullException 
                    data = ms.ToArray();
                }
            }

            RecognizeCelebritiesRequest request = new RecognizeCelebritiesRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = new MemoryStream(data),
                },
            };                

            RecognizeCelebritiesResponse response = rekognitionClient.RecognizeCelebrities(request);

            if (response.CelebrityFaces.Count > 0)
            {
                foreach (var celeb in response.CelebrityFaces)
                {
                    richTextBox1.Text = $"{celeb.Name}";

                    //checkPhoto(response, this.label1.Text);
                }
            }
            else
            {
                richTextBox1.Text = ">>> 닮은 인물이 존재하지 않습니다.";
            }
        }

        private void checkPhoto(RecognizeCelebritiesResponse response, String filename)
        {
            // Load a bitmap to modify with face bounding box rectangles.
            Bitmap facesHighlighted = new Bitmap(filename);
            Pen pen = new Pen(Color.Black, 3);
            Font drawFont = new Font("Arial", 12);

            // Create a graphics context.
            using (var graphics = Graphics.FromImage(facesHighlighted))
            {
                foreach (var fd in response.CelebrityFaces)
                {
                    // Get the bounding box.
                    BoundingBox bb = fd.Face.BoundingBox;
                    Console.WriteLine($"Bounding box = ({bb.Left}, {bb.Top}, {bb.Height}, {bb.Width})");

                    // Draw the rectangle using the bounding box values.
                    // They are percentages so scale them to the picture.
                    graphics.DrawRectangle(
                        pen,
                        x: facesHighlighted.Width * bb.Left,
                        y: facesHighlighted.Height * bb.Top,
                        width: facesHighlighted.Width * bb.Width,
                        height: facesHighlighted.Height * bb.Height);
                    graphics.DrawString(
                        fd.Name,
                        font: drawFont,
                        brush: Brushes.White,
                        x: facesHighlighted.Width * bb.Left,
                        y: (facesHighlighted.Height * bb.Top) + (facesHighlighted.Height * bb.Height));
                }
            }

            // Save the image with highlights as PNG.
            //string fileout = filename.Replace(Path.GetExtension(filename), "_celebrityfaces.png");
            //facesHighlighted.Save(fileout, System.Drawing.Imaging.ImageFormat.Png);
            //pictureBox03.Image = facesHighlighted;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(filterInfoCollection == null || filterInfoCollection.Count < 1)
            {
                MessageBox.Show("카메라등 캡져디바이스가 없습니다.");
                return;
            }

            this.label1.Text = "";
            photoForm pf = new photoForm();

            pf.Owner = this;

            if(pf.ShowDialog() == DialogResult.OK)
            {
                pictureBox01.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox01.Image = ReceivedData;

                getAWSinfo(null, "1");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (filterInfoCollection == null || filterInfoCollection.Count < 1)
            {
                MessageBox.Show("카메라등 캡져디바이스가 없습니다.");
                return;
            }

            this.label2.Text = "";
            photoForm pf = new photoForm();

            pf.Owner = this;

            if (pf.ShowDialog() == DialogResult.OK)
            {
                pictureBox02.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox02.Image = ReceivedData;

                getAWSinfo(null, "2");
            }
        }
    }
}
