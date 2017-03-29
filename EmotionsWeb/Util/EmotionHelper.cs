using EmotionsWeb.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;

using System.Reflection;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Common.Contract;

namespace EmotionsWeb.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient EmoClient;

        public EmotionHelper(string key)
        {
            EmoClient = new EmotionServiceClient(key);
        }

        public  async Task<EmoPicture> detectAndExtractFaceAsync(Stream imgStream) {

            Emotion[] emociones = await EmoClient.RecognizeAsync(imgStream);

            var emoPicture = new EmoPicture();
            emoPicture.Faces = extractFaces(emociones, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> extractFaces(Emotion[] emociones, EmoPicture emoPicture)
        {
            var listFaces = new ObservableCollection<EmoFace>();

            foreach (var emotion in emociones)
            {
                var emoFace = new EmoFace() {
                    x = emotion.FaceRectangle.Left,
                    y = emotion.FaceRectangle.Top,
                    width = emotion.FaceRectangle.Width,
                    height = emotion.FaceRectangle.Height,
                    Picture = emoPicture
                };

               
                emoFace.Emotions = processEmotions((Scores)emotion.Scores, emoFace);
                emoPicture.Faces.Add(emoFace);
            }

            return listFaces;
        }

        private ObservableCollection<EmoEmotion> processEmotions(Scores scores, EmoFace emoFace)
        {
            var listEmotions = new ObservableCollection<EmoEmotion>();

            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //var filterProperties = properties.Where(p => p.PropertyType == typeof(float));
            var filterProperties = from p in properties
                                   where p.PropertyType == typeof(float)
                                   select p;

            var emotype = EmoEmotionEnum.Undetermined;
            foreach (var prop in filterProperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emotype))
                {
                    emotype = EmoEmotionEnum.Undetermined;
                }

                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float)prop.GetValue(scores);
                emoEmotion.EmotionType = emotype;
                emoEmotion.Face = emoFace;

                listEmotions.Add(emoEmotion);
            }


            return listEmotions;
        }
    }
}