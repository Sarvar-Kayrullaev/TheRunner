using UnityEngine;

namespace Code.Scripts.Data.Language
{
    
    public static class Language
    {
        /// <summary>
        /// Retrieves the translated string for a given key and language code.
        /// </summary>
        /// <param name="key">The key for the translation.</param>
        /// <param name="languageCode">The language code to retrieve the translation for.</param>
        /// <returns>The translated string if found, otherwise returns the key.</returns>
        public static string Get(string key, LanguageCode languageCode)
        {
            var data = Data();
            var languageIndex = (int)languageCode;
            foreach (var item in data)
            {
                if (item.Key == key)
                {
                    if (item.Translated.Length > languageIndex)
                    {
                        return item.Translated[languageIndex];
                    }
                    else
                    {
                        Debug.LogWarning($"Translation for '{key}' not found for language index {languageIndex}. Returning key.");
                    }
                }
            }
            Debug.LogWarning($"Key '{key}' not found in language data. Returning key.");
            return key;
        }

        /// <summary>
        /// Initializes the language data.
        /// This method can be used to set up any necessary data structures or configurations for the language system.
        /// /// </summary>
        private static void Words()
        {
            /* List of all data by key by index */
            // (0) "Shop"
            // (1) "Buy"
            // (2) "Equip"
            // (3) "Customize"
            // (4) "Weapons"
            // (5) "Ammo"
            // (6) "Boost"
            // (7) "Skill"
            // (8) "Upgrade"
            // (9) "Handgun"
            // (10) "Shotgun"
            // (11) "SMG"
            // (12) "Rifle"
            // (13) "Sniper"
            // (14) "LMG"
            // (15) "Launcher"
            // (16) "Damage"
            // (17) "Accuracy"
            // (18) "Fire Rate"
            // (19) "Reload Speed"
            // (20) "Mobility"
            // (21) "Back"
            // (22) "Cancel"
            // (23) "Close"
            // (24) "Confirm"
            // (25) "Continue"
            // (26) "Exit"
            // (27) "Settings"
            // (28) "Main Menu"
            // (29) "Language"
            // (30) "Graphics"
            // (31) "Audio"
            // (32) "Controls"
            // (33) "Credits"
            // (34) "About"
            // (35) "Map"
            // (36) "Guide"
            // (37) "Help"
            // (38) "Exit Game"
            // (39) "Play"
            // (40) "Pause"
            // (41) "Resume"
            // (42) "Save"
            // (43) "Load"
            // (44) "Delete"
            // (45) "Empty"
            // (46) "Loading"
            // (47) "Error"
            // (48) "Unlock"
            // (49) "Locked"
            // (50) "Achievements"
            // (51) "Completed"
            // (52) "In Progress"
            // (53) "Not Started"
            // (54) "New Game"
            // (55) "Reloading"
            // (56) "Game Over"
            // (57) "Victory"
            // (58) "Defeat"
            // (59) "Next"
            // (60) "Previous"
            // (61) "Select"
            // (62) "Deselect"
            // (63) "Search"
            // (64) "Captured"
            // (65) "Mission"
            // (66) "Objective"
            // (67) "Challenge"
            // (68) "Reward"
            // (69) "Bonus"
            // (70) "Level"

        }
        /// <summary>
        /// Retrieves the translated string for a given key and language code, with a fallback to English if the translation is not found.
        /// /// </summary>
        public enum LanguageCode
        {
            English,        // 0
            Spanish,        // 1
            French,         // 2
            Russian,        // 3
            German,         // 4
            Italian,        // 5
            Portuguese,     // 6
            Chinese,        // 7
            Japanese,       // 8
            Korean,         // 9
            Arabic,         // 10
            Hindi,          // 11
            Filipino,       // 12
            Turkish,        // 13
            Vietnamese,     // 14
            Thai,           // 15
            Indonesian,     // 16
            Malay,          // 17
            Uzbek,          // 18
            Brazilian,      // 19
        }

        /// <summary>
        /// DataModel represents a translation entry with a key and an array of translated strings.
        /// /// </summary>
        public static DataModel[] Data()
        {
            // FOR VISUAL STUDIO AI ASSEST: If the given word does not translate correctly in another language or is too long, it will be shortened or remain in the default English language.

            DataModel[] data = new DataModel[71];
            data[0] = new()
            {
                Key = "Shop",
                Translated = new[]
                {
                    "Shop",         // English
                    "Tienda",       // Spanish
                    "Boutique",     // French
                    "Tienda",       // Russian
                    "Geschäft",     // German
                    "Negozio",      // Italian
                    "Boutique",     // Portuguese
                    "商店",          // Chinese
                    "ショップ",      // Japanese
                    "상점",         // Korean
                    "متجر",        // Arabic
                    "दुकान",       // Hindi
                    "Tindahan",     // Filipino
                    "Mağaza",       // Turkish
                    "Cửa hàng",     // Vietnamese
                    "ร้านค้า",      // Thai
                    "Toko",         // Indonesian
                    "Kedai",        // Malay
                    "Do'kon",       // Uzbek
                }
            };
            data[1] = new()
            {
                Key = "Buy",
                Translated = new[]
                {
                    "Buy", // English
                    "Comprar", // Spanish
                    "Acheter", // French
                    "Купить", // Russian
                    "Kaufen", // German
                    "Comprare", // Italian
                    "Comprar", // Portuguese
                    "购买", // Chinese
                    "購入", // Japanese
                    "구매", // Korean
                    "شراء", // Arabic
                    "खरीदें", // Hindi
                    "Bumili", // Filipino
                    "Satın Al", // Turkish
                    "Mua", // Vietnamese
                    "ซื้อ", // Thai
                    "Beli", // Indonesian
                    "Beli", // Malay
                    "Sotib olish" // Uzbek
                }
            };
            data[2] = new()
            {
                Key = "Equip",
                Translated = new[]
                {
                    "Equip", // English
                    "Equipar", // Spanish
                    "Équiper", // French
                    "Экипировать", // Russian
                    "Ausrüsten", // German
                    "Equipaggiare", // Italian
                    "Equipar", // Portuguese
                    "装备", // Chinese
                    "装備", // Japanese
                    "장착", // Korean
                    "تجهيز", // Arabic
                    "सज्जित करें", // Hindi
                    "I-equip", // Filipino
                    "Ekipman", // Turkish
                    "Trang bị", // Vietnamese
                    "ติดตั้ง", // Thai
                    "Perlengkapan", // Indonesian
                    "Perlengkapan", // Malay
                    "Jihozlash" // Uzbek
                }
            };
            data[3] = new()
            {
                Key = "Customize",
                Translated = new[]
                {
                    "Customize", // English
                    "Personalizar", // Spanish
                    "Personnaliser", // French
                    "Настроить", // Russian
                    "Anpassen", // German
                    "Personalizzare", // Italian
                    "Personalizar", // Portuguese
                    "自定义", // Chinese
                    "カスタマイズ", // Japanese
                    "사용자 정의", // Korean
                    "تخصيص", // Arabic
                    "अनुकूलित करें", // Hindi
                    "I-customize", // Filipino
                    "Özelleştirmek", // Turkish
                    "Tùy chỉnh", // Vietnamese
                    "ปรับแต่ง", // Thai
                    "Kustomisasi", // Indonesian
                    "Kustomisasi", // Malay
                    "Moslash" // Uzbek
                }
            };
            data[4] = new()
            {
                Key = "Weapons",
                Translated = new[]
                {
                    "Weapons", // English
                    "Armas", // Spanish
                    "Armes", // French
                    "Оружие", // Russian
                    "Waffen", // German
                    "Armi", // Italian
                    "Armas", // Portuguese
                    "武器", // Chinese
                    "武器", // Japanese
                    "무기", // Korean
                    "أسلحة", // Arabic
                    "हथियार", // Hindi
                    "Mga Sandata", // Filipino
                    "Silahlar", // Turkish
                    "Vũ khí", // Vietnamese
                    "อาวุธ", // Thai
                    "Senjata", // Indonesian
                    "Senjata", // Malay
                    "Qurollar" // Uzbek
                }
            };
            data[5] = new()
            {
                Key = "Ammo",
                Translated = new[]
                {
                    "Ammo", // English
                    "Munición", // Spanish
                    "Munitions", // French
                    "Патроны", // Russian
                    "Munition", // German
                    "Munizioni", // Italian
                    "Munição", // Portuguese
                    "弹药", // Chinese
                    "弾薬", // Japanese
                    "탄약", // Korean
                    "ذخيرة", // Arabic
                    "गोला-बारूद", // Hindi
                    "Ammunisyon", // Filipino
                    "Mühimmat", // Turkish
                    "Đạn dược", // Vietnamese
                    "กระสุน", // Thai
                    "Amunisi", // Indonesian
                    "Amunisi", // Malay
                    "O'q-dorilar" // Uzbek
                }
            };
            data[6] = new()
            {
                Key = "Boost",
                Translated = new[]
                {
                    "Boost", // English
                    "Impulso", // Spanish
                    "Boost", // French
                    "Усиление", // Russian
                    "Boost", // German
                    "Potenziamento", // Italian
                    "Impulso", // Portuguese
                    "提升", // Chinese
                    "ブースト", // Japanese
                    "부스트", // Korean
                    "تعزيز", // Arabic
                    "बूस्ट", // Hindi
                    "Pagsulong", // Filipino    
                    "Güçlendirme", // Turkish
                    "Tăng cường", // Vietnamese
                    "บูสต์", // Thai
                    "Dorongan", // Indonesian
                    "Dorongan", // Malay
                    "Kuchaytirish" // Uzbek
                }
            };
            data[7] = new()
            {
                Key = "Skill",
                Translated = new[]
                {
                    "Skill", // English
                    "Habilidad", // Spanish
                    "Compétence", // French
                    "Навык", // Russian
                    "Fähigkeit", // German
                    "Abilità", // Italian
                    "Habilidade", // Portuguese
                    "技能", // Chinese
                    "スキル", // Japanese
                    "기술", // Korean
                    "مهارة", // Arabic
                    "कौशल", // Hindi
                    "Kasanayan", // Filipino
                    "Yetenek", // Turkish
                    "Kỹ năng", // Vietnamese
                    "ทักษะ", // Thai
                    "Keterampilan", // Indonesian
                    "Kemahiran", // Malay
                    "Ko'nikma" // Uzbek
                }
            };
            data[8] = new()
            {
                Key = "Upgrade",
                Translated = new[]
                {
                    "Upgrade", // English
                    "Mejorar", // Spanish
                    "Améliorer", // French  
                    "Улучшение", // Russian
                    "Upgrade", // German
                    "Aggiornamento", // Italian
                    "Atualizar", // Portuguese
                    "升级", // Chinese
                    "アップグレード", // Japanese
                    "업그레이드", // Korean
                    "ترقية", // Arabic
                    "अपग्रेड", // Hindi
                    "Pag-upgrade", // Filipino
                    "Yükseltme", // Turkish
                    "Nâng cấp", // Vietnamese
                    "อัปเกรด", // Thai
                    "Peningkatan", // Indonesian
                    "Peningkatan", // Malay
                    "Yaxshilash" // Uzbek
                }
            };
            data[9] = new()
            {
                Key = "Handgun",
                Translated = new[]
                {
                    "Handgun", // English
                    "Pistola", // Spanish
                    "Pistolet", // French
                    "Пистолет", // Russian
                    "Handfeuerwaffe", // German
                    "Pistola", // Italian
                    "Pistola", // Portuguese
                    "手枪", // Chinese
                    "ハンドガン", // Japanese
                    "권총", // Korean
                    "مسدس", // Arabic
                    "हैंडगन", // Hindi
                    "Baril", // Filipino
                    "Tabanca", // Turkish
                    "Súng lục", // Vietnamese
                    "ปืนพก", // Thai
                    "Pistol", // Indonesian
                    "Pistol", // Malay
                    "Pistolet" // Uzbek
                }
            };
            data[10] = new()
            {
                Key = "Shotgun",
                Translated = new[]
                {

                    "Shotgun", // English
                    "Escopeta", // Spanish
                    "Fusil à pompe", // French
                    "Дробовик", // Russian  
                    "Schrotflinte", // German
                    "Fucile a pompa", // Italian
                    "Espingarda", // Portuguese 
                    "霰弹枪", // Chinese
                    "ショットガン", // Japanese
                    "산탄총", // Korean
                    "بندقية", // Arabic
                    "शॉटगन", // Hindi
                    "Shotgun", // Filipino
                    "Pompalı tüfek", // Turkish
                    "Súng ngắn", // Vietnamese
                    "ปืนลูกซอง", // Thai  
                    "Senapan", // Indonesian
                    "Senapang", // Malay
                    "Shotgun" // Uzbek
                }
            };
            data[11] = new()
            {
                Key = "SMG",
                Translated = new[]
                {
                    "SMG", // English
                    "Ametralladora", // Spanish
                    "Fusil mitrailleur", // French  
                    "ПП", // Russian
                    "Maschinenpistole", // German
                    "Mitralletta", // Italian
                    "Metralhadora", // Portuguese
                    "冲锋枪", // Chinese
                    "サブマシンガン", // Japanese
                    "기관단총", // Korean
                    "رشاش", // Arabic
                    "एसएमजी", // Hindi
                    "SMG", // Filipino
                    "Tüfek", // Turkish
                    "Súng tiểu liên", // Vietnamese
                    "ปืนกลเบา", // Thai
                    "Senapan mesin ringan", // Indonesian
                    "Senapang mesin ringan", // Malay
                    "SMG" // Uzbek
                }
            };
            data[12] = new()
            {
                Key = "Rifle",
                Translated = new[]
                {
                    "Rifle", // English
                    "Rifle", // Spanish 
                    "Fusil", // French
                    "Винтовка", // Russian
                    "Gewehr", // German
                    "Fucile", // Italian
                    "Rifle", // Portuguese (There is no correct translation for this language. It is used as is in English)
                    "步枪", // Chinese
                    "ライフル", // Japanese
                    "소총", // Korean
                    "بندقية", // Arabic
                    "राइफल", // Hindi
                    "Rifle", // Filipino (There is no correct translation for this language. It is used as is in English)
                    "Tüfek", // Turkish
                    "Rifle", // Vietnamese (There is no correct translation for this language. It is used as is in English)
                    "ไรเฟิล", // Thai
                    "Senapan", // Indonesian
                    "Rifle", // Malay (There is no correct translation for this language. It is used as is in English)
                    "Rifle" // Uzbek (There is no correct translation for this language. It is used as is in English)
                }
            };
            data[13] = new()
            {
                Key = "Sniper",
                Translated = new[]
                {
                    "Sniper", // English
                    "Francotirador", // Spanish
                    "Tireur d'élite", // French
                    "Снайпер", // Russian
                    "Scharfschütze", // German
                    "Cecchino", // Italian
                    "Atirador", // Portuguese
                    "狙击手", // Chinese
                    "スナイパー", // Japanese
                    "저격수", // Korean
                    "قناص", // Arabic
                    "स्नाइपर", // Hindi
                    "Sniper", // Filipino
                    "Keskin nişancı", // Turkish
                    "Xạ thủ bắn tỉa", // Vietnamese
                    "มือปืนสไนเปอร์", // Thai
                    "Penembak jitu", // Indonesian
                    "Penembak tepat", // Malay
                    "Snayper" // Uzbek
                }
            };
            data[14] = new()
            {
                Key = "LMG",
                Translated = new[]
                {
                    "LMG", // English
                    "LMG", // Spanish
                    "LMG", // French
                    "Пулемет", // Russian
                    "LMG", // German
                    "LMG", // Italian
                    "LMG", // Portuguese
                    "轻机枪", // Chinese
                    "軽機関銃", // Japanese
                    "경기관총", // Korean
                    "رشاش خفيف", // Arabic
                    "एलएमजी", // Hindi
                    "LMG", // Filipino
                    "LMG", // Turkish
                    "LMG", // Vietnamese
                    "ปืนกลเบา", // Thai
                    "LMG", // Indonesian
                    "LMG", // Malay
                    "LMG" // Uzbek
                }
            };

            data[15] = new()
            {
                Key = "Launcher",
                Translated = new[]
                {
                    "Launcher", // English
                    "Lanzador", // Spanish
                    "Lanceur", // French
                    "Пусковая", // Russian
                    "Raketenwerfer", // German
                    "Lanciatore", // Italian
                    "Lançador", // Portuguese
                    "发射器", // Chinese
                    "ランチャー", // Japanese
                    "발사기", // Korean
                    "قاذفة", // Arabic
                    "लॉन्चर", // Hindi
                    "Lunsad", // Filipino
                    "Fırlatıcı", // Turkish
                    "Máy phóng", // Vietnamese
                    "เครื่องยิง", // Thai
                    "Peluncur", // Indonesian
                    "Pelancar", // Malay
                    "Launcher" // Uzbek
                }
            };
            data[16] = new()
            {
                Key = "Damage",
                Translated = new[]
                {
                    "Damage", // English
                    "Daño", // Spanish
                    "Dégâts", // French
                    "Урон", // Russian
                    "Schaden", // German
                    "Danno", // Italian
                    "Dano", // Portuguese
                    "伤害", // Chinese
                    "ダメージ", // Japanese
                    "데미지", // Korean
                    "ضرر", // Arabic
                    "नुकसान", // Hindi
                    "Pinsala", // Filipino
                    "Hasar", // Turkish
                    "Thiệt hại", // Vietnamese
                    "ความเสียหาย", // Thai
                    "Kerusakan", // Indonesian
                    "Kerusakan", // Malay
                    "Zarar" // Uzbek
                }
            };
            data[17] = new()
            {
                Key = "Accuracy",
                Translated = new[]
                {
                    "Accuracy", // English
                    "Precisión", // Spanish
                    "Précision", // French
                    "Точность", // Russian
                    "Genauigkeit", // German
                    "Precisione", // Italian
                    "Precisão", // Portuguese
                    "准确度", // Chinese
                    "精度", // Japanese
                    "정확도", // Korean
                    "دقة", // Arabic
                    "सटीकता", // Hindi
                    "Katumpakan", // Filipino
                    "Doğruluk", // Turkish
                    "Độ chính xác", // Vietnamese
                    "ความแม่นยำ", // Thai
                    "Akurasi", // Indonesian
                    "Ketepatan", // Malay
                    "Aniqlik" // Uzbek
                }
            };
            data[18] = new()
            {
                Key = "Fire Rate",
                Translated = new[]
                {
                    "Fire Rate", // English
                    "Tasa de Fuego", // Spanish
                    "Taux de Tir", // French
                    "Скорострельность", // Russian
                    "Feuerrate", // German
                    "Velocità di Fuoco", // Italian
                    "Taxa de Fogo", // Portuguese
                    "射速", // Chinese
                    "発射速度", // Japanese
                    "발사 속도", // Korean
                    "معدل إطلاق النار", // Arabic
                    "फायर रेट", // Hindi
                    "Rate ng Apoy", // Filipino
                    "Hızlı Ateş", // Turkish
                    "Tốc độ bắn", // Vietnamese
                    "อัตราการยิง", // Thai
                    "Kecepatan Tembakan", // Indonesian
                    "Kadar Tembakan", // Malay
                    "Otish tezligi" // Uzbek
                }
            };
            data[19] = new()
            {
                Key = "Reload Speed",
                Translated = new[]
                {
                    "Reload Speed", // English
                    "Velocidad de Recarga", // Spanish
                    "Vitesse de Recharge", // French
                    "Скорость Перезарядки", // Russian
                    "Nachladegeschwindigkeit", // German
                    "Velocità di Ricarica", // Italian
                    "Velocidade de Recarga", // Portuguese
                    "换弹速度", // Chinese
                    "リロード速度", // Japanese
                    "재장전 속도", // Korean
                    "سرعة إعادة التحميل", // Arabic
                    "रीलोड गति", // Hindi
                    "Bilis ng Reload", // Filipino
                    "Hızlı Yeniden Yükleme", // Turkish
                    "Tốc độ nạp đạn", // Vietnamese
                    "ความเร็วในการรีโหลด", // Thai
                    "Kecepatan Pengisian Ulang", // Indonesian
                    "Kelajuan Memuat Semula", // Malay
                    "Qayta yuklash tezligi" // Uzbek
                }
            };

            data[20] = new()
            {
                Key = "Mobility",
                Translated = new[]
                {
                    "Mobility", // English
                    "Movilidad", // Spanish
                    "Mobilité", // French
                    "Мобильность", // Russian
                    "Mobilität", // German
                    "Mobilità", // Italian
                    "Mobilidade", // Portuguese
                    "机动性", // Chinese
                    "機動性", // Japanese
                    "기동성", // Korean
                    "تنقل", // Arabic
                    "गतिशीलता", // Hindi
                    "Kakayahang Maglakbay", // Filipino
                    "Hareketlilik", // Turkish
                    "Di động", // Vietnamese
                    "ความคล่องตัว", // Thai
                    "Mobilitas", // Indonesian
                    "Mobiliti", // Malay
                    "Harakatlilik" // Uzbek
                }
            };

            // Most used words in the game interface

            data[21] = new()
            {
                Key = "Back",
                Translated = new[]
                {
                    "Back", // English
                    "Atrás", // Spanish
                    "Retour", // French
                    "Назад", // Russian
                    "Zurück", // German
                    "Indietro", // Italian
                    "Voltar", // Portuguese
                    "返回", // Chinese
                    "戻る", // Japanese
                    "뒤로", // Korean
                    "عودة", // Arabic
                    "वापस", // Hindi
                    "Bumalik", // Filipino
                    "Geri", // Turkish
                    "Quay lại", // Vietnamese
                    "กลับ", // Thai
                    "Kembali", // Indonesian
                    "Kembali", // Malay
                    "Orqaga" // Uzbek
                }
            };
            data[22] = new()
            {
                Key = "Cancel",
                Translated = new[]
                {
                    "Cancel", // English
                    "Cancelar", // Spanish
                    "Annuler", // French
                    "Отмена", // Russian    
                    "Abbrechen", // German
                    "Annulla", // Italian
                    "Cancelar", // Portuguese
                    "取消", // Chinese
                    "キャンセル", // Japanese
                    "취소", // Korean
                    "إلغاء", // Arabic
                    "रद्द करें", // Hindi
                    "Kanselahin", // Filipino
                    "İptal", // Turkish
                    "Hủy", // Vietnamese
                    "ยกเลิก", // Thai
                    "Batalkan", // Indonesian
                    "Batalkan", // Malay
                    "Bekor qilish" // Uzbek
                }
            };
            data[23] = new()
            {
                Key = "Close",
                Translated = new[]
                {
                    "Close", // English
                    "Cerrar", // Spanish
                    "Fermer", // French
                    "Закрыть", // Russian
                    "Schließen", // German
                    "Chiudere", // Italian
                    "Fechar", // Portuguese
                    "关闭", // Chinese
                    "閉じる", // Japanese
                    "닫기", // Korean
                    "إغلاق", // Arabic
                    "बंद करें", // Hindi
                    "Isara", // Filipino
                    "Kapat", // Turkish
                    "Đóng", // Vietnamese
                    "ปิด", // Thai
                    "Tutup", // Indonesian
                    "Tutup", // Malay
                    "Yopish" // Uzbek
                }
            };
            data[24] = new()
            {
                Key = "Confirm",
                Translated = new[]
                {
                    "Confirm", // English
                    "Confirmar", // Spanish
                    "Confirmer", // French
                    "Подтвердить", // Russian
                    "Bestätigen", // German
                    "Confermare", // Italian
                    "Confirmar", // Portuguese
                    "确认", // Chinese
                    "確認", // Japanese
                    "확인", // Korean
                    "تأكيد", // Arabic
                    "पुष्टि करें", // Hindi
                    "Kumpirmahin", // Filipino
                    "Onayla", // Turkish
                    "Xác nhận", // Vietnamese
                    "ยืนยัน", // Thai
                    "Konfirmasi", // Indonesian
                    "Sahkan", // Malay
                    "Tasdiqlash" // Uzbek
                }
            };
            data[25] = new()
            {
                Key = "Continue",
                Translated = new[]
                {
                    "Continue", // English
                    "Continuar", // Spanish
                    "Continuer", // French
                    "Продолжить", // Russian
                    "Fortsetzen", // German
                    "Continuare", // Italian
                    "Continuar", // Portuguese
                    "继续", // Chinese
                    "続ける", // Japanese
                    "계속", // Korean
                    "متابعة", // Arabic
                    "जारी रखें", // Hindi
                    "Magpatuloy", // Filipino
                    "Devam Et", // Turkish
                    "Tiếp tục", // Vietnamese
                    "ต่อไป", // Thai
                    "Lanjutkan", // Indonesian
                    "Teruskan", // Malay
                    "Davom etish" // Uzbek
                }
            };
            data[26] = new()
            {
                Key = "Exit",
                Translated = new[]
                {
                    "Exit", // English
                    "Salir", // Spanish
                    "Quitter", // French
                    "Выход", // Russian
                    "Beenden", // German
                    "Uscire", // Italian
                    "Sair", // Portuguese
                    "退出", // Chinese
                    "終了", // Japanese
                    "종료", // Korean
                    "خروج", // Arabic
                    "बाहर निकलें", // Hindi
                    "Lumabas", // Filipino
                    "Çıkış", // Turkish
                    "Thoát", // Vietnamese
                    "ออก", // Thai
                    "Keluar", // Indonesian
                    "Keluar", // Malay
                    "Chiqish" // Uzbek
                }
            };
            data[27] = new()
            {
                Key = "Settings",
                Translated = new[]
                {
                    "Settings", // English
                    "Configuración", // Spanish
                    "Paramètres", // French
                    "Настройки", // Russian
                    "Einstellungen", // German
                    "Impostazioni", // Italian
                    "Configurações", // Portuguese
                    "设置", // Chinese
                    "設定", // Japanese
                    "설정", // Korean
                    "إعدادات", // Arabic
                    "सेटिंग्स", // Hindi
                    "Mga Setting", // Filipino
                    "Ayarlar", // Turkish
                    "Cài đặt", // Vietnamese
                    "การตั้งค่า", // Thai
                    "Pengaturan", // Indonesian
                    "Tetapan", // Malay
                    "Sozlamalar" // Uzbek
                }
            };
            data[28] = new()
            {
                Key = "Main Menu",
                Translated = new[]
                {
                    "Main Menu", // English
                    "Menú Principal", // Spanish
                    "Menu Principal", // French
                    "Главное Меню", // Russian  
                    "Hauptmenü", // German
                    "Menu Principale", // Italian
                    "Menu Principal", // Portuguese
                    "主菜单", // Chinese
                    "メインメニュー", // Japanese
                    "메인 메뉴", // Korean
                    "القائمة الرئيسية", // Arabic
                    "मुख्य मेनू", // Hindi
                    "Pangunahing Menu", // Filipino
                    "Ana Menü", // Turkish
                    "Menu Chính", // Vietnamese
                    "เมนูหลัก", // Thai
                    "Menu Utama", // Indonesian
                    "Menu Utama", // Malay
                    "Asosiy menyu" // Uzbek
                }
            };
            data[29] = new()
            {
                Key = "Language",
                Translated = new[]
                {
                    "Language", // English
                    "Idioma", // Spanish
                    "Langue", // French
                    "Язык", // Russian
                    "Sprache", // German
                    "Lingua", // Italian
                    "Idioma", // Portuguese
                    "语言", // Chinese
                    "言語", // Japanese
                    "언어", // Korean
                    "لغة", // Arabic
                    "भाषा", // Hindi
                    "Wika", // Filipino
                    "Dil", // Turkish
                    "Ngôn ngữ", // Vietnamese
                    "ภาษา", // Thai
                    "Bahasa", // Indonesian
                    "Bahasa", // Malay
                    "Til" // Uzbek
                }
            };
            data[30] = new()
            {
                Key = "Graphics",
                Translated = new[]
                {
                    "Graphics", // English
                    "Gráficos", // Spanish
                    "Graphiques", // French
                    "Графика", // Russian
                    "Grafik", // German
                    "Grafica", // Italian
                    "Gráficos", // Portuguese
                    "图形", // Chinese
                    "グラフィックス", // Japanese
                    "그래픽", // Korean
                    "رسومات", // Arabic
                    "ग्राफिक्स", // Hindi
                    "Mga Grapiko", // Filipino
                    "Grafikler", // Turkish
                    "Đồ họa", // Vietnamese
                    "กราฟิก", // Thai
                    "Grafis", // Indonesian
                    "Grafik", // Malay
                    "Grafika" // Uzbek
                }
            };
            data[31] = new()
            {
                Key = "Audio",
                Translated = new[]
                {
                    "Audio", // English
                    "Audio", // Spanish
                    "Audio", // French
                    "Аудио", // Russian
                    "Audio", // German
                    "Audio", // Italian
                    "Áudio", // Portuguese
                    "音频", // Chinese
                    "オーディオ", // Japanese
                    "오디오", // Korean
                    "صوت", // Arabic
                    "ऑडियो", // Hindi
                    "Audio", // Filipino
                    "Ses", // Turkish
                    "Âm thanh", // Vietnamese
                    "เสียง", // Thai
                    "Audio", // Indonesian
                    "Audio", // Malay
                    "Audio" // Uzbek
                }
            };
            data[32] = new()
            {
                Key = "Controls",
                Translated = new[]
                {
                    "Controls", // English
                    "Controles", // Spanish
                    "Contrôles", // French
                    "Управление", // Russian
                    "Steuerung", // German
                    "Controlli", // Italian
                    "Controles", // Portuguese
                    "控制", // Chinese
                    "コントロール", // Japanese
                    "컨트롤", // Korean
                    "التحكم", // Arabic
                    "नियंत्रण", // Hindi
                    "Mga Kontrol", // Filipino
                    "Kontroller", // Turkish
                    "Điều khiển", // Vietnamese
                    "การควบคุม", // Thai
                    "Kontrol", // Indonesian
                    "Kawalan", // Malay
                    "Boshqaruv" // Uzbek
                }
            };
            data[33] = new()
            {
                Key = "Credits",
                Translated = new[]
                {
                    "Credits", // English
                    "Créditos", // Spanish
                    "Crédits", // French
                    "Кредиты", // Russian
                    "Credits", // German
                    "Crediti", // Italian
                    "Créditos", // Portuguese
                    "致谢", // Chinese
                    "クレジット", // Japanese
                    "크레딧", // Korean
                    "الاعتمادات", // Arabic
                    "क्रेडिट", // Hindi
                    "Mga Kredito", // Filipino
                    "Krediler", // Turkish
                    "Tín dụng", // Vietnamese
                    "เครดิต", // Thai
                    "Kredit", // Indonesian
                    "Kredit", // Malay
                    "Kreditlar" // Uzbek
                }
            };
            data[34] = new()
            {
                Key = "About",
                Translated = new[]
                {
                    "About", // English
                    "Acerca de", // Spanish
                    "À propos", // French
                    "О программе", // Russian
                    "Über", // German
                    "Informazioni", // Italian
                    "Sobre", // Portuguese
                    "关于", // Chinese
                    "について", // Japanese
                    "정보", // Korean
                    "حول", // Arabic
                    "के बारे में", // Hindi
                    "Tungkol sa", // Filipino
                    "Hakkında", // Turkish
                    "Về", // Vietnamese
                    "เกี่ยวกับ", // Thai
                    "Tentang", // Indonesian
                    "Mengenai", // Malay
                    "Haqida" // Uzbek
                }
            };
            data[35] = new()
            {
                Key = "Map",
                Translated = new[]
                {
                    "Map", // English
                    "Mapa", // Spanish
                    "Carte", // French  
                    "Карта", // Russian
                    "Karte", // German
                    "Mappa", // Italian
                    "Mapa", // Portuguese
                    "地图", // Chinese
                    "地図", // Japanese
                    "지도", // Korean
                    "خريطة", // Arabic
                    "मानचित्र", // Hindi
                    "Mapa", // Filipino
                    "Harita", // Turkish
                    "Bản đồ", // Vietnamese
                    "แผนที่", // Thai
                    "Peta", // Indonesian
                    "Peta", // Malay
                    "Xarita" // Uzbek
                }
            };
            data[36] = new()
            {
                Key = "Guide",
                Translated = new[]
                {
                    "Guide", // English
                    "Guía", // Spanish
                    "Guide", // French
                    "Руководство", // Russian
                    "Anleitung", // German
                    "Guida", // Italian
                    "Guia", // Portuguese
                    "指南", // Chinese
                    "ガイド", // Japanese
                    "가이드", // Korean
                    "دليل", // Arabic
                    "गाइड", // Hindi
                    "Gabay", // Filipino
                    "Rehber", // Turkish
                    "Hướng dẫn", // Vietnamese
                    "คู่มือ", // Thai
                    "Panduan", // Indonesian
                    "Panduan", // Malay
                    "Qo'llanma" // Uzbek
                }
            };
            data[37] = new()
            {
                Key = "Help",
                Translated = new[]
                {
                    "Help", // English
                    "Ayuda", // Spanish
                    "Aide", // French
                    "Помощь", // Russian
                    "Hilfe", // German
                    "Aiuto", // Italian
                    "Ajuda", // Portuguese
                    "帮助", // Chinese
                    "ヘルプ", // Japanese
                    "도움말", // Korean
                    "مساعدة", // Arabic
                    "मदद", // Hindi
                    "Tulong", // Filipino
                    "Yardım", // Turkish
                    "Trợ giúp", // Vietnamese
                    "ช่วยเหลือ", // Thai
                    "Bantuan", // Indonesian
                    "Bantuan", // Malay
                    "Yordam" // Uzbek
                }
            };
            data[38] = new()
            {
                Key = "Exit Game",
                Translated = new[]
                {
                    "Exit Game", // English
                    "Salir del Juego", // Spanish
                    "Quitter le Jeu", // French
                    "Выйти из Игры", // Russian
                    "Spiel Beenden", // German
                    "Uscire dal Gioco", // Italian
                    "Sair do Jogo", // Portuguese
                    "退出游戏", // Chinese
                    "ゲームを終了", // Japanese
                    "게임 종료", // Korean
                    "خروج من اللعبة", // Arabic
                    "गेम से बाहर निकलें", // Hindi
                    "Lumabas sa Laro", // Filipino
                    "Oyundan Çık", // Turkish
                    "Thoát Trò Chơi", // Vietnamese
                    "ออกจากเกม", // Thai
                    "Keluar dari Permainan", // Indonesian
                    "Keluar Permainan", // Malay
                    "O'yindan chiqish" // Uzbek
                }
            };
            data[39] = new()
            {
                Key = "Play",
                Translated = new[]
                {
                    "Play", // English
                    "Jugar", // Spanish
                    "Jouer", // French
                    "Играть", // Russian
                    "Spielen", // German
                    "Giocare", // Italian
                    "Jogar", // Portuguese
                    "玩", // Chinese
                    "プレイ", // Japanese
                    "플레이", // Korean
                    "العب", // Arabic
                    "खेलें", // Hindi
                    "Maglaro", // Filipino
                    "Oyna", // Turkish
                    "Chơi", // Vietnamese
                    "เล่น", // Thai
                    "Bermain", // Indonesian
                    "Main", // Malay
                    "O'ynash" // Uzbek
                }
            };
            data[40] = new()
            {
                Key = "Pause",
                Translated = new[]
                {
                    "Pause", // English
                    "Pausa", // Spanish
                    "Pause", // French
                    "Пауза", // Russian
                    "Pause", // German
                    "Pausa", // Italian
                    "Pausa", // Portuguese
                    "暂停", // Chinese
                    "ポーズ", // Japanese
                    "일시 정지", // Korean
                    "إيقاف مؤقت", // Arabic
                    "रोकें", // Hindi
                    "Pahinga", // Filipino
                    "Duraklatma", // Turkish
                    "Tạm dừng", // Vietnamese
                    "พัก", // Thai
                    "Jeda", // Indonesian
                    "Jeda", // Malay
                    "To'xtatish" // Uzbek
                }
            };
            data[41] = new()
            {
                Key = "Resume",
                Translated = new[]
                {
                    "Resume", // English
                    "Reanudar", // Spanish
                    "Reprendre", // French
                    "Продолжить", // Russian
                    "Fortsetzen", // German
                    "Riprendere", // Italian
                    "Retomar", // Portuguese
                    "继续", // Chinese
                    "再開", // Japanese
                    "재개", // Korean
                    "استئناف", // Arabic
                    "जारी रखें", // Hindi
                    "Ipatuloy", // Filipino
                    "Devam Et", // Turkish
                    "Tiếp tục", // Vietnamese
                    "ดำเนินการต่อ", // Thai
                    "Lanjutkan", // Indonesian
                    "Teruskan", // Malay
                    "Davom etish" // Uzbek
                }
            };
            data[42] = new()
            {
                Key = "Save",
                Translated = new[]
                {
                    "Save", // English
                    "Guardar", // Spanish
                    "Enregistrer", // French
                    "Сохранить", // Russian
                    "Speichern", // German
                    "Salvare", // Italian
                    "Salvar", // Portuguese
                    "保存", // Chinese
                    "保存", // Japanese
                    "저장", // Korean
                    "حفظ", // Arabic
                    "सहेजें", // Hindi
                    "I-save", // Filipino
                    "Kaydet", // Turkish
                    "Lưu", // Vietnamese
                    "บันทึก", // Thai
                    "Simpan", // Indonesian
                    "Simpan", // Malay
                    "Saqlash" // Uzbek
                }
            };
            data[43] = new()
            {
                Key = "Load",
                Translated = new[]
                {
                    "Load", // English
                    "Cargar", // Spanish
                    "Charger", // French
                    "Загрузить", // Russian
                    "Laden", // German
                    "Caricare", // Italian
                    "Carregar", // Portuguese
                    "加载", // Chinese
                    "ロード", // Japanese
                    "불러오기", // Korean
                    "تحميل", // Arabic
                    "लोड करें", // Hindi
                    "I-load", // Filipino
                    "Yükle", // Turkish
                    "Tải", // Vietnamese
                    "โหลด", // Thai
                    "Muat", // Indonesian
                    "Muat", // Malay
                    "Yuklash" // Uzbek
                }
            };
            data[44] = new()
            {
                Key = "Delete",
                Translated = new[]
                {
                    "Delete", // English
                    "Eliminar", // Spanish
                    "Supprimer", // French
                    "Удалить", // Russian
                    "Löschen", // German
                    "Eliminare", // Italian
                    "Excluir", // Portuguese
                    "删除", // Chinese
                    "削除", // Japanese
                    "삭제", // Korean
                    "حذف", // Arabic
                    "हटाएं", // Hindi
                    "Tanggalin", // Filipino
                    "Sil", // Turkish
                    "Xóa", // Vietnamese
                    "ลบ", // Thai
                    "Hapus", // Indonesian
                    "Padamkan", // Malay
                    "O'chirish" // Uzbek
                }
            };
            data[45] = new()
            {
                Key = "Empty",
                Translated = new[]
                {
                    "Empty", // English
                    "Vacío", // Spanish
                    "Vide", // French
                    "Пусто", // Russian
                    "Leer", // German
                    "Vuoto", // Italian
                    "Vazio", // Portuguese
                    "空", // Chinese
                    "空", // Japanese
                    "비어 있음", // Korean
                    "فارغ", // Arabic
                    "खाली", // Hindi
                    "Walang laman", // Filipino
                    "Boş", // Turkish
                    "Trống", // Vietnamese
                    "ว่างเปล่า", // Thai
                    "Kosong", // Indonesian
                    "Kosong", // Malay
                    "Bo'sh" // Uzbek
                }
            };
            data[46] = new()
            {
                Key = "Loading",
                Translated = new[]
                {
                    "Loading", // English
                    "Cargando", // Spanish
                    "Chargement", // French
                    "Загрузка", // Russian
                    "Laden", // German
                    "Caricamento", // Italian
                    "Carregando", // Portuguese
                    "加载中", // Chinese
                    "読み込み中", // Japanese
                    "로딩 중", // Korean
                    "تحميل", // Arabic
                    "लोड हो रहा है", // Hindi
                    "Naglo-load", // Filipino
                    "Yükleniyor", // Turkish
                    "Đang tải", // Vietnamese
                    "กำลังโหลด", // Thai
                    "Memuat", // Indonesian
                    "Memuatkan", // Malay
                    "Yuklanmoqda" // Uzbek
                }
            };
            data[47] = new()
            {
                Key = "Error",
                Translated = new[]
                {
                    "Error", // English
                    "Error", // Spanish
                    "Erreur", // French
                    "Ошибка", // Russian
                    "Fehler", // German
                    "Errore", // Italian
                    "Erro", // Portuguese
                    "错误", // Chinese
                    "エラー", // Japanese
                    "오류", // Korean
                    "خطأ", // Arabic
                    "त्रुटि", // Hindi
                    "Error", // Filipino
                    "Hata", // Turkish
                    "Lỗi", // Vietnamese
                    "ข้อผิดพลาด", // Thai
                    "Ralat", // Indonesian
                    "Ralat", // Malay
                    "Xato" // Uzbek
                }
            };
            data[48] = new()
            {
                Key = "Unlock",
                Translated = new[]
                {
                    "Unlock", // English
                    "Desbloquear", // Spanish
                    "Déverrouiller", // French
                    "Разблокировать", // Russian
                    "Freischalten", // German
                    "Sbloccare", // Italian
                    "Desbloquear", // Portuguese
                    "解锁", // Chinese
                    "アンロック", // Japanese
                    "잠금 해제", // Korean
                    "فتح القفل", // Arabic
                    "अनलॉक करें", // Hindi
                    "I-unlock", // Filipino
                    "Kilitlemeyi Aç", // Turkish
                    "Mở khóa", // Vietnamese
                    "ปลดล็อก", // Thai
                    "Buka kunci", // Indonesian
                    "Buka Kunci", // Malay
                    "Qulfni ochish" // Uzbek
                }
            };
            data[49] = new()
            {
                Key = "Locked",
                Translated = new[]
                {
                    "Locked", // English
                    "Bloqueado", // Spanish
                    "Verrouillé", // French
                    "Заблокировано", // Russian
                    "Gesperrt", // German
                    "Bloccato", // Italian
                    "Bloqueado", // Portuguese
                    "已锁定", // Chinese
                    "ロック中", // Japanese
                    "잠김", // Korean
                    "مغلق", // Arabic
                    "लॉक किया गया है", // Hindi
                    "Naka-lock", // Filipino
                    "Kilitli", // Turkish
                    "Đã khóa", // Vietnamese
                    "ถูกล็อก", // Thai
                    "Terkunci", // Indonesian
                    "Terkunci", // Malay
                    "Qulflangan" // Uzbek
                }
            };
            data[50] = new()
            {
                Key = "Achievements",
                Translated = new[]
                {
                    "Achievements", // English
                    "Logros", // Spanish
                    "Succès", // French
                    "Достижения", // Russian
                    "Errungenschaften", // German
                    "Traguardi", // Italian
                    "Conquistas", // Portuguese
                    "成就", // Chinese
                    "実績", // Japanese
                    "업적", // Korean
                    "إنجازات", // Arabic
                    "उपलब्धियां", // Hindi
                    "Mga Tagumpay", // Filipino
                    "Başarılar", // Turkish
                    "Thành tựu", // Vietnamese
                    "ความสำเร็จ", // Thai
                    "Prestasi", // Indonesian
                    "Pencapaian", // Malay
                    "Yutuqlar" // Uzbek
                }
            };
            data[51] = new()
            {
                Key = "Completed",
                Translated = new[]
                {
                    "Completed", // English
                    "Completado", // Spanish
                    "Terminé", // French
                    "Завершено", // Russian
                    "Abgeschlossen", // German
                    "Completato", // Italian
                    "Concluído", // Portuguese
                    "已完成", // Chinese
                    "完了", // Japanese
                    "완료됨", // Korean
                    "مكتمل", // Arabic
                    "पूर्ण हुआ", // Hindi
                    "Nakumpleto", // Filipino
                    "Tamamlandı", // Turkish
                    "Đã hoàn thành", // Vietnamese
                    "เสร็จสมบูรณ์", // Thai
                    "Selesai", // Indonesian
                    "Selesai", // Malay
                    "Bajarilgan" // Uzbek
                }
            };
            data[52] = new()
            {
                Key = "In Progress",
                Translated = new[]
                {
                    "In Progress", // English
                    "En Progreso", // Spanish
                    "En Cours", // French
                    "В процессе", // Russian
                    "In Bearbeitung", // German
                    "In Corso", // Italian
                    "Em Andamento", // Portuguese
                    "进行中", // Chinese
                    "進行中", // Japanese
                    "진행 중", // Korean
                    "قيد التقدم", // Arabic
                    "प्रगति पर", // Hindi
                    "Sa Progreso", // Filipino
                    "Devam Ediyor", // Turkish
                    "Đang Tiến Hành", // Vietnamese
                    "กำลังดำเนินการ", // Thai
                    "Sedang Berlangsung", // Indonesian
                    "Sedang Berlangsung", // Malay
                    "Jarayonda" // Uzbek
                }
            };
            data[53] = new()
            {
                Key = "Not Started",
                Translated = new[]
                {
                    "Not Started", // English
                    "No Iniciado", // Spanish
                    "Non Commencé", // French
                    "Не Начато", // Russian
                    "Nicht Gestartet", // German
                    "Non Iniziato", // Italian
                    "Não Iniciado", // Portuguese
                    "未开始", // Chinese
                    "未開始", // Japanese
                    "시작되지 않음", // Korean
                    "لم يبدأ", // Arabic
                    "शुरू नहीं हुआ", // Hindi
                    "Hindi Pa Nagsimula", // Filipino
                    "Başlanmadı", // Turkish
                    "Chưa Bắt Đầu", // Vietnamese
                    "ยังไม่เริ่ม", // Thai
                    "Belum Dimulai", // Indonesian
                    "Belum Bermula", // Malay
                    "Boshlanmagan" // Uzbek
                }
            };
            data[54] = new()
            {
                Key = "New Game",
                Translated = new[]
                {
                    "New Game", // English
                    "Nuevo Juego", // Spanish
                    "Nouveau Jeu", // French
                    "Новая Игра", // Russian
                    "Neues Spiel", // German
                    "Nuovo Gioco", // Italian
                    "Novo Jogo", // Portuguese
                    "新游戏", // Chinese
                    "新しいゲーム", // Japanese
                    "새 게임", // Korean
                    "لعبة جديدة", // Arabic
                    "नया खेल", // Hindi
                    "Bagong Laro", // Filipino
                    "Yeni Oyun", // Turkish
                    "Trò Chơi Mới", // Vietnamese
                    "เกมใหม่", // Thai
                    "Permainan Baru", // Indonesian
                    "Permainan Baru", // Malay
                    "Yangi O'yin" // Uzbek
                }
            };
            data[55] = new ()
            {
                Key = "Reloading",
                Translated = new[]
                {
                    "Reloading", // English
                    "Recargando", // Spanish
                    "Rechargement", // French
                    "Перезарядка", // Russian
                    "Nachladen", // German
                    "Ricaricamento", // Italian
                    "Recarregando", // Portuguese
                    "重新加载", // Chinese
                    "リロード中", // Japanese
                    "재장전", // Korean
                    "إعادة تحميل", // Arabic
                    "रिलोडिंग", // Hindi
                    "Nag-reload", // Filipino
                    "Yeniden Yükleme", // Turkish
                    "Đang Tải Lại", // Vietnamese
                    "กำลังรีโหลด", // Thai
                    "Memuat Ulang", // Indonesian
                    "Muat Semula", // Malay
                    "Qayta yuklash" // Uzbek
                }
            };
            data[56] = new ()
            {
                Key = "Game Over",
                Translated = new[]
                {
                    "Game Over", // English
                    "Fin del Juego", // Spanish
                    "Fin du Jeu", // French
                    "Конец Игры", // Russian
                    "Spiel Vorbei", // German
                    "Fine del Gioco", // Italian
                    "Fim do Jogo", // Portuguese
                    "游戏结束", // Chinese
                    "ゲームオーバー", // Japanese
                    "게임 오버", // Korean
                    "انتهت اللعبة", // Arabic
                    "गेम ओवर", // Hindi
                    "Laro Tapos", // Filipino
                    "Oyun Bitti", // Turkish
                    "Trò Chơi Kết Thúc", // Vietnamese
                    "เกมจบ", // Thai
                    "Permainan Tamat", // Indonesian
                    "Permainan Tamat", // Malay
                    "O'yin tugadi" // Uzbek
                }
            };
            data[57] = new()
            {
                Key = "Victory",
                Translated = new[]
                {
                    "Victory", // English
                    "Victoria", // Spanish
                    "Victoire", // French
                    "Победа", // Russian
                    "Sieg", // German
                    "Vittoria", // Italian
                    "Vitória", // Portuguese
                    "胜利", // Chinese
                    "勝利", // Japanese
                    "승리", // Korean
                    "نصر", // Arabic
                    "जीत", // Hindi
                    "Tagumpay", // Filipino
                    "Zafer", // Turkish
                    "Chiến Thắng", // Vietnamese
                    "ชัยชนะ", // Thai
                    "Kemenangan", // Indonesian
                    "Kemenangan", // Malay
                    "G'alaba" // Uzbek
                }
            };
            data[58] = new()
            {
                Key = "Defeat",
                Translated = new[]
                {
                    "Defeat", // English
                    "Derrière", // Spanish
                    "Défaite", // French
                    "Поражение", // Russian
                    "Verlust", // German
                    "Defeat", // Italian
                    "Derrota", // Portuguese
                    "失敗", // Chinese
                    "失敗", // Japanese
                    "패배", // Korean
                    "خسارة", // Arabic
                    "패", // Hindi
                    "Mali", // Filipino
                    "Kesalahan", // Turkish
                    "Thua", // Vietnamese
                    "แพง", // Thai
                    "Menang", // Indonesian
                    "Menang", // Malay
                    "Mag'lubiyat" // Uzbek
                }
            };

            return data;
        }

        public class DataModel
        {
            public string Key { get; set; }
            public string[] Translated { get; set; }
        }
    }
}
