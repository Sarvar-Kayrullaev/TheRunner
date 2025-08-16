using System;
using System.Collections;
using System.Threading.Tasks;
using Code.Scripts.Data.Language;
using Data;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Code.Scripts.GUI.Adapter
{
    
    public class FontAdapter : MonoBehaviour
    {
        // regular
        [HideInInspector] public TMP_FontAsset robotoFontRegular;
        [HideInInspector] public TMP_FontAsset arabicFontRegular;
        [HideInInspector] public TMP_FontAsset indianFontRegular;
        [HideInInspector] public TMP_FontAsset japanFontRegular;
        [HideInInspector] public TMP_FontAsset koreanFontRegular;
        [HideInInspector] public TMP_FontAsset russianFontRegular;
        [HideInInspector] public TMP_FontAsset thaiFontRegular;
        [HideInInspector] public TMP_FontAsset notosansFontRegular;
        [HideInInspector] public TMP_FontAsset chinesenFontRegular;
        
        // medium
        [HideInInspector] public TMP_FontAsset robotoFontMedium;
        [HideInInspector] public TMP_FontAsset arabicFontMedium;
        [HideInInspector] public TMP_FontAsset indianFontMedium;
        [HideInInspector] public TMP_FontAsset japanFontMedium;
        [HideInInspector] public TMP_FontAsset koreanFontMedium;
        [HideInInspector] public TMP_FontAsset russianFontMedium;
        [HideInInspector] public TMP_FontAsset thaiFontMedium;
        [HideInInspector] public TMP_FontAsset notosansFontMedium;
        [HideInInspector] public TMP_FontAsset chinesenFontMedium;
        
        // bold
        [HideInInspector] public TMP_FontAsset robotoFontBold;
        [HideInInspector] public TMP_FontAsset arabicFontBold;
        [HideInInspector] public TMP_FontAsset indianFontBold;
        [HideInInspector] public TMP_FontAsset japanFontBold;
        [HideInInspector] public TMP_FontAsset koreanFontBold;
        [HideInInspector] public TMP_FontAsset russianFontBold;
        [HideInInspector] public TMP_FontAsset thaiFontBold;
        [HideInInspector] public TMP_FontAsset notosansFontBold;
        [HideInInspector] public TMP_FontAsset chinesenFontBold;
        
        // Complex
        private TMP_FontAsset titleFontRegular;
        private TMP_FontAsset titleFontMedium;
        private TMP_FontAsset titleFontBold;
        private TMP_FontAsset titleFontBlack;
        
        private TMP_FontAsset descriptionFontLight;
        private TMP_FontAsset descriptionFontRegular;
        private TMP_FontAsset descriptionFontMedium;
        private TMP_FontAsset descriptionFontBold;
        private TMP_FontAsset descriptionFontBlack;
        
        private TMP_FontAsset readableFont;
        private TMP_FontAsset headFont;
        
        


        private void Awake()
        {
            titleFontRegular = Resources.Load<TMP_FontAsset>("Fonts/FontBarlowCondensed_Title/BarlowCondensed-Regular SDF");
            titleFontMedium = Resources.Load<TMP_FontAsset>("Fonts/FontBarlowCondensed_Title/BarlowCondensed-Medium SDF");
            titleFontBold = Resources.Load<TMP_FontAsset>("Fonts/FontBarlowCondensed_Title/BarlowCondensed-Bold SDF");
            titleFontBlack = Resources.Load<TMP_FontAsset>("Fonts/FontBarlowCondensed_Title/BarlowCondensed-Black SDF");

            descriptionFontLight = Resources.Load<TMP_FontAsset>("Fonts/FontOutfit_Description/Outfit-Light SDF");
            descriptionFontRegular = Resources.Load<TMP_FontAsset>("Fonts/FontOutfit_Description/Outfit-Regular SDF");
            descriptionFontMedium = Resources.Load<TMP_FontAsset>("Fonts/FontOutfit_Description/Outfit-Medium SDF");
            descriptionFontBold = Resources.Load<TMP_FontAsset>("Fonts/FontOutfit_Description/Outfit-Bold SDF");
            descriptionFontBlack = Resources.Load<TMP_FontAsset>("Fonts/FontOutfit_Description/Outfit-Black SDF");

            readableFont = Resources.Load<TMP_FontAsset>("Fonts/FontRobotoFlex_Readable/RobotoFlex SDF");
            headFont = Resources.Load<TMP_FontAsset>("Fonts/FontBebasNeue_Big/BebasNeue-Regular SDF");
            
            
            // Roboto
            robotoFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Roboto/roboto_regular SDF");
            robotoFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Roboto/roboto_medium SDF");
            robotoFontBold = Resources.Load<TMP_FontAsset>("Fonts/Roboto/roboto_bold SDF");
            
            //Arabic
            arabicFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Arabic/arabic_regular SDF");
            arabicFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Arabic/arabic_medium SDF");
            arabicFontBold = Resources.Load<TMP_FontAsset>("Fonts/Arabic/arabic_bold SDF");
            
            // indian
            indianFontRegular = Resources.Load<TMP_FontAsset>( "Fonts/Hindi/hindi_regular SDF");
            indianFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Hindi/hindi_medium SDF");
            indianFontBold = Resources.Load<TMP_FontAsset>("Fonts/Hindi/hindi_bold SDF");
            
            // japan
            japanFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Japan/japan_regular SDF");
            japanFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Japan/japan_medium SDF");
            japanFontBold = Resources.Load<TMP_FontAsset>("Fonts/Japan/japan_bold SDF");
            
            // korean
            koreanFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Korean/korean_regular SDF");
            koreanFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Korean/korean_medium SDF");
            koreanFontBold = Resources.Load<TMP_FontAsset>("Fonts/Korean/korean_bold SDF");
            
            // russian
            russianFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Russian/russian_regular SDF");
            russianFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Russian/russian_medium SDF");
            russianFontBold = Resources.Load<TMP_FontAsset>("Fonts/Russian/russian_bold SDF");
            
            // thai
            thaiFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Thai/thai_regular SDF");
            thaiFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Thai/thai_medium SDF");
            thaiFontBold = Resources.Load<TMP_FontAsset>("Fonts/Thai/thai_bold SDF");
            
            // notosans
            notosansFontRegular = Resources.Load<TMP_FontAsset>("Fonts/NotoSans/notosans_regular SDF");
            notosansFontMedium = Resources.Load<TMP_FontAsset>("Fonts/NotoSans/notosans_medium SDF");
            notosansFontBold = Resources.Load<TMP_FontAsset>("Fonts/NotoSans/notosans_bold SDF");
            
            // chinese
            chinesenFontRegular = Resources.Load<TMP_FontAsset>("Fonts/Chinese/chinese_regular SDF");
            chinesenFontMedium = Resources.Load<TMP_FontAsset>("Fonts/Chinese/chinese_medium SDF");
            chinesenFontBold = Resources.Load<TMP_FontAsset>("Fonts/Chinese/chinese_bold SDF");


        }
        

        public TMP_FontAsset GetFont(FontType type,FontStyle style, out Language.LanguageCode languageCode)
        {
            var settings = DataProvider.LoadSettingsData();
            languageCode = settings.Options.Language;
            
            return settings.Options.Language switch
            {
                Language.LanguageCode.English => GetComplexFont(type, style),
                Language.LanguageCode.Arabic => GetArabicFont(type),
                Language.LanguageCode.Hindi => GetIndianFont(type),
                Language.LanguageCode.Japanese => GetJapanFont(type),
                Language.LanguageCode.Korean => GetKoreanFont(type),
                Language.LanguageCode.Russian => GetRussianFont(type),
                Language.LanguageCode.Thai => GetThaiFont(type),
                Language.LanguageCode.Chinese => GetChinesenFont(type),
                Language.LanguageCode.Brazilian => GetRussianFont(type),
                Language.LanguageCode.French => GetComplexFont(type, style),
                Language.LanguageCode.German => GetComplexFont(type, style),
                Language.LanguageCode.Filipino => GetComplexFont(type, style),
                Language.LanguageCode.Indonesian => GetComplexFont(type, style),
                Language.LanguageCode.Italian => GetComplexFont(type, style),
                Language.LanguageCode.Malay => GetComplexFont(type, style),
                Language.LanguageCode.Spanish => GetComplexFont(type, style),
                Language.LanguageCode.Portuguese => GetComplexFont(type, style),
                Language.LanguageCode.Turkish => GetComplexFont(type,style),
                Language.LanguageCode.Vietnamese => GetNotosansFont(type),
                Language.LanguageCode.Uzbek => GetComplexFont(type, style),
                _ => GetComplexFont(type, style)
            };
        }

        private TMP_FontAsset GetComplexFont(FontType type, FontStyle style)
        {
            switch(type)
            {
                case FontType.Default:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontRegular,
                        FontStyle.Description => descriptionFontRegular,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontRegular
                    };
                }
                case FontType.Light:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontRegular,
                        FontStyle.Description => descriptionFontLight,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontRegular
                    };
                }
                case FontType.Regular:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontRegular,
                        FontStyle.Description => descriptionFontRegular,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontRegular
                    };
                }
                case FontType.Medium:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontMedium,
                        FontStyle.Description => descriptionFontMedium,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontMedium
                    };
                }
                case FontType.Bold:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontBold,
                        FontStyle.Description => descriptionFontBold,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontBold
                    };
                }
                case FontType.Black:
                {
                    return style switch
                    {
                        FontStyle.Title => titleFontBlack,
                        FontStyle.Description => descriptionFontBlack,
                        FontStyle.Head => headFont,
                        FontStyle.Readable => readableFont,
                        _ => robotoFontBold
                    };
                }
            };
            return null;
        }

        private TMP_FontAsset GetArabicFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => arabicFontRegular,
                FontType.Medium => arabicFontMedium,
                FontType.Bold => arabicFontBold,
                FontType.Black => arabicFontBold,
                _ => arabicFontRegular
            };
        }

        private TMP_FontAsset GetIndianFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => indianFontRegular,
                FontType.Medium => indianFontMedium,
                FontType.Bold => indianFontBold,
                FontType.Black => indianFontBold,
                _ => indianFontRegular
            };
        }

        private TMP_FontAsset GetJapanFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => japanFontRegular,
                FontType.Medium => japanFontMedium,
                FontType.Bold => japanFontBold,
                FontType.Black => japanFontBold,
                _ => japanFontRegular
            };
        }

        private TMP_FontAsset GetKoreanFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => koreanFontRegular,
                FontType.Medium => koreanFontMedium,
                FontType.Bold => koreanFontBold,
                FontType.Black => koreanFontBold,
                _ => koreanFontRegular
            };
        }

        private TMP_FontAsset GetRussianFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => russianFontRegular,
                FontType.Medium => russianFontMedium,
                FontType.Bold => russianFontBold,
                FontType.Black => russianFontBold,
                _ => russianFontRegular
            };
        }

        private TMP_FontAsset GetThaiFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => thaiFontRegular,
                FontType.Medium => thaiFontMedium,
                FontType.Bold => thaiFontBold,
                FontType.Black => thaiFontBold,
                _ => thaiFontRegular
            };
        }

        private TMP_FontAsset GetNotosansFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => notosansFontRegular,
                FontType.Medium => notosansFontMedium,
                FontType.Bold => notosansFontBold,
                FontType.Black => notosansFontBold,
                _ => notosansFontRegular
            };
        }

        private TMP_FontAsset GetChinesenFont(FontType type)
        {
            return type switch
            {
                FontType.Regular => chinesenFontRegular,
                FontType.Medium => chinesenFontMedium,
                FontType.Bold => chinesenFontBold,
                FontType.Black => chinesenFontBold,
                _ => chinesenFontRegular
            };
        }
    }
}
