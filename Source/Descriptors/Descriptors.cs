using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Descriptors
    {
        public Descriptors(FormMain fm)
        {
            FormMain.Descriptors = this;

            // 
            MaxLevelSkill = 3;

            //
            XmlDocument xmlDoc;

            // Загружаем конфигурацию игры
            Descriptor.Descriptors = this;
            CellOfMenu.Descriptors = this;

            // Загрузка компьютерных игроков
            xmlDoc = CreateXmlDocument("Config\\ComputerPlayers.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/ComputerPlayers/ComputerPlayer"))
            {
                ComputerPlayers.Add(new ComputerPlayer(n));
            }

            // Загрузка игроков-людей
            if (File.Exists(Program.FolderResources + "Players.xml"))
            {
                xmlDoc = CreateXmlDocument("Players.xml");
                foreach (XmlNode n in xmlDoc.SelectNodes("/Players/Player"))
                {
                    HumanPlayers.Add(new HumanPlayer(n));
                }
                AutoCreatedPlayer = false;
            }
            else
            {
                AddHumanPlayer("Игрок");
                AutoCreatedPlayer = true;
            }

            // Загрузка страниц столицы
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\CapitalPages.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/CapitalPage"))
            {
                CapitalPages.Add(new CapitalPage(n));
            }

            // Загрузка типов ландшафта
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\TypeLandscapes.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/TypeLandscape"))
            {
                TypeLandscapes.Add(new DescriptorTypeLandscape(n));
            }

            // Загрузка конфигурации базовых ресурсов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\BaseResources.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/BaseResource"))
            {
                BaseResources.Add(new DescriptorBaseResource(n));
            }

            Debug.Assert(BaseResources.Count > 0);
            BaseResources.Capacity = BaseResources.Count;
            Gold = FindBaseResource(FormMain.Config.NameResourceGold);

            // Загрузка конфигураций лобби
            xmlDoc = CreateXmlDocument("Config\\TypeLobby.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeLobbies/TypeLobby"))
            {
                TypeLobbies.Add(new TypeLobby(n));
            }

            // Загрузка стартовых бонусов
            xmlDoc = CreateXmlDocument(@"Config\StartBonus.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/StartBonuses/StartBonus"))
            {
                StartBonuses.Add(new StartBonus(n));
            }

            // Загрузка конфигурации свойств существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\PropertiesCreature.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/PropertyCreature"))
            {
                PropertiesCreature.Add(new DescriptorProperty(n));
            }

            // Загрузка конфигурации типов сооружений
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\TypeConstructions.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/TypeConstruction"))
            {
                TypeConstructions.Add(new DescriptorTypeConstruction(n));
            }

            // Загрузка конфигурации сооружений
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Constructions.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Construction"))
            {
                DescriptorConstruction dc = new DescriptorConstruction(n);
                Constructions.Add(dc);
            }

            // Загрузка ресурсов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Resources.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Resource"))
            {
                Resources.Add(new DescriptorResource(n));
            }

            // Загрузка групп предметов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\GroupItems.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/GroupItems"))
            {
                GroupItems.Add(new DescriptorGroupItems(n));
            }

            // Загрузка предметов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Items.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Item"))
            {
                Items.Add(new DescriptorItem(n));
            }

            // Загрузка конфигурации причин смерти существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\ReasonsOfDeath.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/ReasonOfDeath"))
            {
                ReasonsOfDeath.Add(new DescriptorReasonOfDeath(n));
            }

            // Загрузка конфигурации ядов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Poisons.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Poison"))
            {
                Poisons.Add(new DescriptorPoison(n));
            }

            // Загрузка конфигурации типов атаки
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\TypeAttacks.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/TypeAttack"))
            {
                TypeAttacks.Add(new DescriptorAttack(n));
            }

            // Загрузка конфигурации потребностей существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\NeedsCreature.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/NeedCreature"))
            {
                NeedsCreature.Add(new DescriptorNeed(n));
            }

            // Загрузка конфигурации интересов существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\InterestCreature.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/InterestCreature"))
            {
                InterestCreature.Add(new DescriptorInterest(n));
            }

            // Загрузка конфигурации перков
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Perks.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Perk"))
            {
                Perks.Add(new DescriptorPerk(n));
            }

            // Загрузка конфигурации типов способностей
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\TypeAbilities.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/TypeAbility"))
            {
                TypeAbilities.Add(new DescriptorTypeAbility(n));
            }

            // Загрузка конфигурации способностей
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Abilities.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Ability"))
            {
                Abilities.Add(new DescriptorAbility(n));
            }

            // Загрузка конфигурации специализаций
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Specializations.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Specialization"))
            {
                Specializations.Add(new DescriptorSpecialization(n));
            }

            // Загрузка конфигурации вторичных навыков
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\SecondarySkills.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/SecondarySkill"))
            {
                SecondarySkills.Add(new DescriptorSecondarySkill(n));
            }

            // Загрузка конфигурации состояний существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\StatesCreature.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/StateCreature"))
            {
                StatesCreature.Add(new DescriptorStateCreature(n));
            }

            // Загрузка конфигурации типов существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\TypeCreatures.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/TypeCreature"))
            {
                TypeCreatures.Add(new DescriptorTypeCreature(n));
            }

            // Загрузка конфигурации существ
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\Creatures.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/Creature"))
            {
                Creatures.Add(new DescriptorCreature(n));
            }

            // Загрузка конфигурации уровней налогов
            xmlDoc = CreateXmlDocument(@"Config\Descriptors\LevelTaxes.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Descriptors/LevelTax"))
            {
                LevelTaxes.Add(new DescriptorLevelTax(n));
            }

            DefaultLevelTax = FindLevelTax(FormMain.Config.NameDefaultLevelTax);

            // Настраиваем связи
            foreach (DescriptorTypeLandscape tl in TypeLandscapes)
                tl.TuneLinks();

            foreach (DescriptorPoison p in Poisons)
                p.TuneLinks();

            foreach (DescriptorTypeAbility ta in TypeAbilities)
                ta.TuneLinks();

            foreach (DescriptorAbility a in Abilities)
                a.TuneLinks();

            foreach (DescriptorSpecialization s in Specializations)
                s.TuneLinks();

            foreach (DescriptorSecondarySkill ss in SecondarySkills)
                ss.TuneLinks();

            foreach (DescriptorResource dr in Resources)
                dr.TuneLinks();

            foreach (DescriptorItem i in Items)
                i.TuneLinks();

            foreach (DescriptorGroupItems i in GroupItems)
                i.TuneLinks();

            foreach (DescriptorProperty pc in PropertiesCreature)
                pc.TuneLinks();

            foreach (DescriptorNeed pc in NeedsCreature)
                pc.TuneLinks();

            foreach (DescriptorCreature tc in Creatures)
                tc.TuneLinks();

            foreach (DescriptorBaseResource br in BaseResources)
                br.TuneLinks();

            foreach (DescriptorTypeConstruction tc in TypeConstructions)
                tc.TuneLinks();

            foreach (DescriptorConstructionVisitSimple cv in ConstructionsVisits)
                cv.TuneLinks();

            foreach (DescriptorConstruction c in Constructions)
                c.TuneLinks();

            foreach (TypeLobby tl in TypeLobbies)
                tl.TuneDeferredLinks();

            foreach (DescriptorItem i in Items)
                i.AfterTuneLinks();

            foreach (DescriptorConstruction c in Constructions)
                c.AfterTuneLinks();

            //
            ReasonOfDeathInBattle = FindReasonOfDeath(FormMain.Config.IDReasonOfDeathInBattle);

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Program.FolderResources + pathToXml);
                return doc;
            }
        }

        internal List<DescriptorTypeLandscape> TypeLandscapes { get; } = new List<DescriptorTypeLandscape>();
        internal List<TypeLobby> TypeLobbies { get; } = new List<TypeLobby>();
        internal List<StartBonus> StartBonuses { get; } = new List<StartBonus>();
        internal List<ComputerPlayer> ComputerPlayers { get; } = new List<ComputerPlayer>();
        internal List<HumanPlayer> HumanPlayers { get; } = new List<HumanPlayer>();
        internal List<CapitalPage> CapitalPages { get; } = new List<CapitalPage>();
        internal bool AutoCreatedPlayer { get; }

        // Списки описателей
        internal SortedList<string, DescriptorEntity> Entities { get; } = new SortedList<string, DescriptorEntity>();// Список всех сущностей
        internal DescriptorBaseResource Gold { get; }
        internal List<DescriptorBaseResource> BaseResources { get; } = new List<DescriptorBaseResource>();

        // Товары в сооружениях
        internal List<DescriptorProduct> ConstructionProducts { get; } = new List<DescriptorProduct>();

        // Сооружения
        internal List<DescriptorTypeConstruction> TypeConstructions { get; } = new List<DescriptorTypeConstruction>();
        internal List<DescriptorConstructionVisitSimple> ConstructionsVisits { get; } = new List<DescriptorConstructionVisitSimple>();
        internal List<DescriptorConstruction> Constructions { get; } = new List<DescriptorConstruction>();

        // Существа
        internal List<DescriptorReasonOfDeath> ReasonsOfDeath { get; } = new List<DescriptorReasonOfDeath>();
        internal List<DescriptorPoison> Poisons { get; } = new List<DescriptorPoison>();
        internal List<DescriptorAttack> TypeAttacks { get; } = new List<DescriptorAttack>();
        internal List<DescriptorPerk> Perks { get; } = new List<DescriptorPerk>();
        internal List<DescriptorTypeAbility> TypeAbilities { get; } = new List<DescriptorTypeAbility>();
        internal List<DescriptorAbility> Abilities { get; } = new List<DescriptorAbility>();
        internal List<DescriptorSpecialization> Specializations { get; } = new List<DescriptorSpecialization>();
        internal List<DescriptorSecondarySkill> SecondarySkills { get; } = new List<DescriptorSecondarySkill>();
        internal List<DescriptorStateCreature> StatesCreature { get; } = new List<DescriptorStateCreature>();
        internal List<DescriptorProperty> PropertiesCreature { get; } = new List<DescriptorProperty>();
        internal List<DescriptorNeed> NeedsCreature { get; } = new List<DescriptorNeed>();
        internal List<DescriptorInterest> InterestCreature { get; } = new List<DescriptorInterest>();
        internal List<DescriptorTypeCreature> TypeCreatures { get; } = new List<DescriptorTypeCreature>();
        internal List<DescriptorCreature> Creatures { get; } = new List<DescriptorCreature>();
        internal List<DescriptorGroupItems> GroupItems { get; } = new List<DescriptorGroupItems>();
        internal List<DescriptorResource> Resources { get; } = new List<DescriptorResource>();
        internal List<DescriptorItem> Items { get; } = new List<DescriptorItem>();
        internal int MaxLevelSkill { get; }

        //
        internal List<DescriptorLevelTax> LevelTaxes { get; } = new List<DescriptorLevelTax>();
        internal DescriptorLevelTax DefaultLevelTax { get; }

        //
        internal List<string> ExternalAvatars { get; } = new List<string>();

        //
        private List<(string, Bitmap)> Textures = new List<(string, Bitmap)>();

        internal DescriptorReasonOfDeath ReasonOfDeathInBattle { get; }

        //
        internal CapitalPage FindCapitalPage(string ID)
        {
            foreach (CapitalPage cp in CapitalPages)
            {
                if (cp.ID == ID)
                    return cp;
            }

            throw new Exception($"Страница столицы {ID} не найдена.");
        }

        internal DescriptorConstructionVisitSimple FindConstructionVisit(string ID)
        {
            foreach (DescriptorConstructionVisitSimple dcv in ConstructionsVisits)
            {
                if (dcv.ID == ID)
                    return dcv;
            }

            throw new Exception($"Посещение {ID} не найдено.");
        }

        internal DescriptorConstruction FindConstruction(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorConstruction dc in Constructions)
            {
                if (dc.ID == ID)
                    return dc;
            }
            if (mustBeExists)
                throw new Exception("Сооружение " + ID + " не найдено.");

            return null;
        }

        internal DescriptorReasonOfDeath FindReasonOfDeath(string ID)
        {
            foreach (DescriptorReasonOfDeath r in ReasonsOfDeath)
            {
                if (r.ID == ID)
                    return r;
            }

            throw new Exception("Причина смерти " + ID + " не найдена.");
        }

        internal DescriptorResource FindResource(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorResource r in Resources)
            {
                if (r.ID == ID)
                    return r;
            }

            if (mustBeExists)
                throw new Exception("Ресурс " + ID + " не найден.");

            return null;
        }

        internal DescriptorItem FindItem(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorItem i in Items)
            {
                if (i.ID == ID)
                    return i;
            }

            if (mustBeExists)
                throw new Exception("Предмет " + ID + " не найден.");

            return null;
        }

        internal DescriptorAttack FindTypeAttack(string ID)
        {
            Debug.Assert(ID.Length > 0);

            foreach (DescriptorAttack ta in TypeAttacks)
            {
                if (ta.ID == ID)
                    return ta;
            }

            throw new Exception("Тип атаки " + ID + " не найден.");
        }

        internal DescriptorBaseResource FindBaseResource(string ID)
        {
            foreach (DescriptorBaseResource br in BaseResources)
            {
                if (br.ID == ID)
                    return br;
            }

            throw new Exception($"Базовый ресурс {ID} не найден.");
        }

        internal DescriptorTypeConstruction FindTypeConstruction(string ID)
        {
            foreach (DescriptorTypeConstruction tc in TypeConstructions)
            {
                if (tc.ID == ID)
                    return tc;
            }

            throw new Exception($"Тип сооружения {ID} не найден.");
        }

        internal DescriptorProduct FindProduct(string ID)
        {
            foreach (DescriptorProduct p in ConstructionProducts)
            {
                if (p.ID == ID)
                    return p;
            }

            throw new Exception($"Товар {ID} не найден.");
        }

        internal DescriptorTypeAbility FindTypeAbility(string ID)
        {
            foreach (DescriptorTypeAbility ta in TypeAbilities)
            {
                if (ta.ID == ID)
                    return ta;
            }

            throw new Exception($"Тип способности {ID} не найден.");
        }

        internal DescriptorPerk FindPerk(string ID)
        {
            foreach (DescriptorPerk p in Perks)
            {
                if (p.ID == ID)
                    return p;
            }

            throw new Exception($"Перк {ID} не найден.");
        }

        internal DescriptorAbility FindAbility(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorAbility a in Abilities)
            {
                if (a.ID == ID)
                    return a;
            }

            if (mustBeExists)
                throw new Exception("Способность " + ID + " не найдена.");

            return null;
        }

        internal DescriptorGroupItems FindGroupItem(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorGroupItems gi in GroupItems)
                if (gi.ID == ID)
                    return gi;

            if (mustBeExists)
                throw new Exception("Группа предметов " + ID + " не найдена.");

            return null;
        }

        internal DescriptorStateCreature FindStateCreature(string ID)
        {
            foreach (DescriptorStateCreature sc in StatesCreature)
            {
                if (sc.ID == ID)
                    return sc;
            }

            throw new Exception("Состояние существа " + ID + " не найдено.");
        }

        internal DescriptorProperty FindPropertyCreature(string ID)
        {
            foreach (DescriptorProperty pc in PropertiesCreature)
            {
                if (pc.ID == ID)
                    return pc;
            }

            throw new Exception("Свойство существа " + ID + " не найдено.");
        }

        internal DescriptorNeed FindNeedCreature(string ID)
        {
            foreach (DescriptorNeed nc in NeedsCreature)
            {
                if (nc.ID == ID)
                    return nc;
            }

            throw new Exception($"Потребность существа {ID} не найдена.");
        }

        internal DescriptorInterest FindInterestCreature(string ID)
        {
            foreach (DescriptorInterest di in InterestCreature)
            {
                if (di.ID == ID)
                    return di;
            }

            throw new Exception($"Интерес существа {ID} не найден.");
        }

        internal DescriptorTypeCreature FindTypeCreature(string ID)
        {
            foreach (DescriptorTypeCreature tu in TypeCreatures)
            {
                if (tu.ID == ID)
                    return tu;
            }

            throw new Exception("Тип существа " + ID + " не найден.");
        }

        internal DescriptorSpecialization FindSpecialization(string ID)
        {
            foreach (DescriptorSpecialization s in Specializations)
            {
                if (s.ID == ID)
                    return s;
            }

            throw new Exception("Специализация " + ID + " не найдена.");
        }

        internal DescriptorSecondarySkill FindSecondarySkill(string ID)
        {
            foreach (DescriptorSecondarySkill ss in SecondarySkills)
            {
                if (ss.ID == ID)
                    return ss;
            }

            throw new Exception("Вторичный навык " + ID + " не найден.");
        }

        internal DescriptorCreature FindCreature(string ID, bool mustBeExists = true)
        {
            foreach (DescriptorCreature tc in Creatures)
            {
                if (tc.ID == ID)
                    return tc;
            }

            if (mustBeExists)
                throw new Exception("Существо " + ID + " не найден.");

            return null;
        }

        internal DescriptorTypeLandscape FindTypeLandscape(string ID)
        {
            foreach (DescriptorTypeLandscape tt in TypeLandscapes)
            {
                if (tt.ID == ID)
                    return tt;
            }

            throw new Exception("Тип ландшафта " + ID + " не найден.");
        }

        internal DescriptorLevelTax FindLevelTax(string ID)
        {
            foreach (DescriptorLevelTax lt in LevelTaxes)
            {
                if (lt.ID == ID)
                    return lt;
            }

            throw new Exception("Уровень налогов " + ID + " не найден.");
        }

        internal void AddVisit(DescriptorConstructionVisitSimple visit)
        {
            ConstructionsVisits.Add(visit);
        }

        internal void AddEntity(DescriptorEntity entity)
        {
            Debug.Assert(!Entities.ContainsKey(entity.ID));

            Entities.Add(entity.ID, entity);
        }

        internal DescriptorEntity FindEntity(string id)
        {
            if (!Entities.TryGetValue(id, out DescriptorEntity entity))
                throw new Exception($"Сущность {id} не найдена.");

            return entity;
        }

        internal void AddHumanPlayer(string name)
        {
            string id;
            bool exist;
            for (int i = 1; ; i++)
            {
                exist = false;
                id = $"HumanPlayer{i}";
                foreach (HumanPlayer hp in HumanPlayers)
                {
                    if (hp.ID == id)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                    break;
            }

            int imageIndex;
            for (imageIndex = 0; ; imageIndex++)
            {
                exist = false;
                foreach (HumanPlayer hp in HumanPlayers)
                {
                    if (hp.ImageIndex == imageIndex)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                    break;
            }

            HumanPlayers.Add(new HumanPlayer(id, name, "-", imageIndex + FormMain.Config.ImageIndexFirstAvatar));

            SaveHumanPlayers();
        }

        internal void SaveHumanPlayers()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.FolderResources + "Players.xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;

            textWriter.WriteStartElement("Players");

            // Записываем информацию об игроках
            foreach (HumanPlayer hp in HumanPlayers)
            {
                hp.SaveToXml(textWriter);
            }

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }

        internal bool CheckNonExistsNamePlayer(string name)
        {
            foreach (ComputerPlayer cp in ComputerPlayers)
                if (cp.Name == name)
                    return false;

            foreach (HumanPlayer hp in HumanPlayers)
                if (hp.Name == name)
                    return false;

            return true;
        }
    }
}