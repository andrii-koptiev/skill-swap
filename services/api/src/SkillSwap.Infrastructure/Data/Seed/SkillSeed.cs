using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data.Seed;

/// <summary>
/// Seeding data for Skills entity
/// </summary>
public static class SkillSeed
{
    /// <summary>
    /// Seeds skills data for development and testing
    /// </summary>
    public static async Task SeedSkillsAsync(SkillSwapDbContext context)
    {
        if (await context.Skills.AnyAsync())
        {
            return; // Skills already seeded
        }

        var skills = new List<Skill>();

        // Technology Skills
        skills.AddRange(CreateTechnologySkills());

        // Creative Skills  
        skills.AddRange(CreateCreativeSkills());

        // Business Skills
        skills.AddRange(CreateBusinessSkills());

        // Language Skills
        skills.AddRange(CreateLanguageSkills());

        // Health & Fitness Skills
        skills.AddRange(CreateHealthSkills());

        // Culinary Skills
        skills.AddRange(CreateCulinarySkills());

        // Crafts & Hobbies Skills
        skills.AddRange(CreateCraftsSkills());

        // Education & Teaching Skills
        skills.AddRange(CreateEducationSkills());

        // Music Skills
        skills.AddRange(CreateMusicSkills());

        // Sports & Recreation Skills
        skills.AddRange(CreateSportsSkills());

        // Science Skills
        skills.AddRange(CreateScienceSkills());

        // Other Skills
        skills.AddRange(CreateOtherSkills());

        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();
    }

    private static List<Skill> CreateTechnologySkills()
    {
        return new List<Skill>
        {
            new("JavaScript", "Modern JavaScript programming including ES6+ features, async/await, and frameworks", SkillCategory.Technology),
            new("Python", "Python programming for web development, data science, automation, and machine learning", SkillCategory.Technology),
            new("React", "Building interactive user interfaces with React hooks, state management, and component architecture", SkillCategory.Technology),
            new("Node.js", "Server-side JavaScript development with Express, APIs, and microservices", SkillCategory.Technology),
            new("C#", "C# programming for desktop, web, and enterprise applications using .NET framework", SkillCategory.Technology),
            new("Java", "Java programming for enterprise applications, Android development, and backend systems", SkillCategory.Technology),
            new("SQL", "Database design, queries, optimization, and stored procedures for relational databases", SkillCategory.Technology),
            new("Git", "Version control with Git, branching strategies, collaboration workflows, and GitHub/GitLab", SkillCategory.Technology),
            new("Docker", "Containerization, Docker compose, orchestration, and deployment strategies", SkillCategory.Technology),
            new("AWS", "Amazon Web Services cloud computing, deployment, and infrastructure management", SkillCategory.Technology),
            new("Angular", "Building scalable web applications with Angular framework and TypeScript", SkillCategory.Technology),
            new("Vue.js", "Progressive JavaScript framework for building user interfaces and single-page applications", SkillCategory.Technology),
            new("Machine Learning", "ML algorithms, data preprocessing, model training, and deployment with Python/R", SkillCategory.Technology),
            new("Cybersecurity", "Network security, ethical hacking, vulnerability assessment, and security protocols", SkillCategory.Technology),
            new("DevOps", "CI/CD pipelines, automation, infrastructure as code, and deployment strategies", SkillCategory.Technology),
            new("iOS Development", "Native iOS app development with Swift, UIKit, and SwiftUI frameworks", SkillCategory.Technology),
            new("Android Development", "Native Android app development with Java/Kotlin and Android Studio", SkillCategory.Technology),
            new("Data Analysis", "Statistical analysis, data visualization, and insights extraction using various tools", SkillCategory.Technology),
            new("Blockchain", "Cryptocurrency, smart contracts, DeFi, and distributed ledger technologies", SkillCategory.Technology),
            new("UI/UX Design", "User interface design, user experience research, prototyping, and usability testing", SkillCategory.Technology)
        };
    }

    private static List<Skill> CreateCreativeSkills()
    {
        return new List<Skill>
        {
            new("Digital Art", "Digital illustration, concept art, character design using Photoshop, Procreate, and other tools", SkillCategory.Creative),
            new("Photography", "Portrait, landscape, street photography, lighting techniques, and photo editing", SkillCategory.Creative),
            new("Video Editing", "Video production, editing with Premiere Pro, Final Cut, color grading, and storytelling", SkillCategory.Creative),
            new("Graphic Design", "Logo design, branding, print design, and visual communication principles", SkillCategory.Creative),
            new("3D Modeling", "3D modeling with Blender, Maya, character rigging, and animation techniques", SkillCategory.Creative),
            new("Creative Writing", "Fiction writing, poetry, screenwriting, character development, and storytelling techniques", SkillCategory.Creative),
            new("Web Design", "Responsive web design, CSS animations, user interface design, and design systems", SkillCategory.Creative),
            new("Animation", "2D/3D animation, motion graphics, character animation, and visual effects", SkillCategory.Creative),
            new("Music Production", "Beat making, mixing, mastering, sound design using DAWs like Ableton, Logic Pro", SkillCategory.Creative),
            new("Painting", "Oil painting, watercolor, acrylic techniques, color theory, and composition", SkillCategory.Creative),
            new("Drawing", "Pencil drawing, sketching, figure drawing, perspective, and artistic fundamentals", SkillCategory.Creative),
            new("Interior Design", "Space planning, color schemes, furniture selection, and design aesthetics", SkillCategory.Creative),
            new("Fashion Design", "Clothing design, pattern making, textile knowledge, and fashion illustration", SkillCategory.Creative),
            new("Calligraphy", "Hand lettering, traditional calligraphy, brush lettering, and typography design", SkillCategory.Creative),
            new("Pottery", "Ceramic arts, wheel throwing, glazing techniques, and kiln firing", SkillCategory.Creative),
            new("Jewelry Making", "Metalworking, beadwork, wire wrapping, and jewelry design principles", SkillCategory.Creative),
            new("Woodworking", "Furniture making, carving, joinery techniques, and tool usage", SkillCategory.Creative),
            new("Sculpture", "Clay modeling, stone carving, metal sculpture, and three-dimensional art", SkillCategory.Creative),
            new("Printmaking", "Screen printing, lithography, etching, and various printing techniques", SkillCategory.Creative),
            new("Textile Arts", "Weaving, dyeing, fabric design, and fiber arts techniques", SkillCategory.Creative)
        };
    }

    private static List<Skill> CreateBusinessSkills()
    {
        return new List<Skill>
        {
            new("Project Management", "Agile, Scrum, planning, resource allocation, risk management, and team coordination", SkillCategory.Business),
            new("Digital Marketing", "SEO, SEM, social media marketing, content strategy, and online advertising", SkillCategory.Business),
            new("Sales", "Lead generation, customer relationship management, negotiation, and closing techniques", SkillCategory.Business),
            new("Accounting", "Financial record keeping, bookkeeping, tax preparation, and financial analysis", SkillCategory.Business),
            new("Public Speaking", "Presentation skills, speech delivery, audience engagement, and communication techniques", SkillCategory.Business),
            new("Leadership", "Team management, strategic thinking, decision making, and organizational development", SkillCategory.Business),
            new("Negotiation", "Conflict resolution, deal making, persuasion techniques, and win-win strategies", SkillCategory.Business),
            new("Content Writing", "Blog writing, copywriting, technical writing, and content strategy development", SkillCategory.Business),
            new("Customer Service", "Client relations, problem solving, communication, and service excellence", SkillCategory.Business),
            new("E-commerce", "Online store management, marketplace selling, inventory management, and fulfillment", SkillCategory.Business),
            new("Financial Planning", "Investment strategies, retirement planning, budgeting, and wealth management", SkillCategory.Business),
            new("Human Resources", "Recruitment, employee relations, performance management, and HR policies", SkillCategory.Business),
            new("Market Research", "Consumer behavior analysis, data collection, trend analysis, and competitive intelligence", SkillCategory.Business),
            new("Business Strategy", "Strategic planning, market analysis, competitive positioning, and growth strategies", SkillCategory.Business),
            new("Entrepreneurship", "Startup development, business model design, funding, and venture creation", SkillCategory.Business),
            new("Supply Chain Management", "Logistics, procurement, vendor management, and operations optimization", SkillCategory.Business),
            new("Quality Assurance", "Process improvement, quality control, testing methodologies, and compliance", SkillCategory.Business),
            new("Real Estate", "Property investment, market analysis, sales techniques, and property management", SkillCategory.Business),
            new("Insurance", "Risk assessment, policy analysis, claims processing, and insurance sales", SkillCategory.Business),
            new("Consulting", "Problem solving, analytical thinking, client advisory, and business improvement", SkillCategory.Business)
        };
    }

    private static List<Skill> CreateLanguageSkills()
    {
        return new List<Skill>
        {
            new("English", "English language proficiency, grammar, conversation, business English, and writing", SkillCategory.Language),
            new("Spanish", "Spanish language learning, conversation practice, grammar, and cultural understanding", SkillCategory.Language),
            new("French", "French language instruction, pronunciation, grammar, and francophone culture", SkillCategory.Language),
            new("German", "German language learning, grammar structures, conversation, and cultural context", SkillCategory.Language),
            new("Mandarin Chinese", "Chinese language learning, characters, pronunciation, and cultural awareness", SkillCategory.Language),
            new("Japanese", "Japanese language instruction, hiragana, katakana, kanji, and cultural understanding", SkillCategory.Language),
            new("Italian", "Italian language learning, conversation, grammar, and Italian culture", SkillCategory.Language),
            new("Portuguese", "Portuguese language instruction, Brazilian and European variants, and culture", SkillCategory.Language),
            new("Russian", "Russian language learning, Cyrillic alphabet, grammar, and cultural context", SkillCategory.Language),
            new("Arabic", "Arabic language instruction, script, grammar, and Middle Eastern culture", SkillCategory.Language),
            new("Korean", "Korean language learning, Hangul, grammar structures, and Korean culture", SkillCategory.Language),
            new("Hindi", "Hindi language instruction, Devanagari script, and Indian cultural context", SkillCategory.Language),
            new("Dutch", "Dutch language learning, grammar, conversation, and Netherlands culture", SkillCategory.Language),
            new("Swedish", "Swedish language instruction, pronunciation, and Scandinavian culture", SkillCategory.Language),
            new("Polish", "Polish language learning, grammar, pronunciation, and Polish culture", SkillCategory.Language),
            new("Turkish", "Turkish language instruction, grammar structures, and Turkish culture", SkillCategory.Language),
            new("Greek", "Greek language learning, alphabet, grammar, and Greek culture", SkillCategory.Language),
            new("Hebrew", "Hebrew language instruction, script, grammar, and cultural understanding", SkillCategory.Language),
            new("Translation", "Professional translation services, interpretation, and cross-cultural communication", SkillCategory.Language),
            new("English as Second Language", "ESL instruction, language acquisition techniques, and cultural adaptation", SkillCategory.Language)
        };
    }

    private static List<Skill> CreateHealthSkills()
    {
        return new List<Skill>
        {
            new("Personal Training", "Fitness coaching, exercise program design, strength training, and health assessment", SkillCategory.Health),
            new("Yoga", "Hatha, Vinyasa, Ashtanga yoga instruction, meditation, and mindfulness practices", SkillCategory.Health),
            new("Nutrition", "Meal planning, dietary advice, nutritional counseling, and healthy eating habits", SkillCategory.Health),
            new("Mental Health Counseling", "Therapy techniques, stress management, emotional support, and psychological wellness", SkillCategory.Health),
            new("Massage Therapy", "Swedish, deep tissue, sports massage, relaxation techniques, and body work", SkillCategory.Health),
            new("First Aid", "Emergency response, CPR, wound care, and basic medical assistance", SkillCategory.Health),
            new("Physical Therapy", "Rehabilitation exercises, injury recovery, mobility improvement, and pain management", SkillCategory.Health),
            new("Meditation", "Mindfulness meditation, breathing techniques, stress reduction, and mental clarity", SkillCategory.Health),
            new("Pilates", "Core strengthening, body alignment, flexibility, and movement coordination", SkillCategory.Health),
            new("Weight Loss Coaching", "Diet planning, exercise routines, motivation, and lifestyle changes", SkillCategory.Health),
            new("Sports Medicine", "Injury prevention, athletic performance, recovery techniques, and sports nutrition", SkillCategory.Health),
            new("Herbalism", "Natural remedies, plant medicine, herbal preparations, and holistic health", SkillCategory.Health),
            new("Acupuncture", "Traditional Chinese medicine, energy healing, and alternative therapy", SkillCategory.Health),
            new("Dance Therapy", "Movement therapy, emotional expression, body awareness, and therapeutic dance", SkillCategory.Health),
            new("Sleep Coaching", "Sleep hygiene, insomnia treatment, relaxation techniques, and sleep optimization", SkillCategory.Health),
            new("Addiction Counseling", "Recovery support, behavioral change, relapse prevention, and addiction therapy", SkillCategory.Health),
            new("Health Coaching", "Lifestyle modification, wellness planning, goal setting, and health education", SkillCategory.Health),
            new("Occupational Therapy", "Daily living skills, adaptive techniques, workplace ergonomics, and rehabilitation", SkillCategory.Health),
            new("Chiropractic Care", "Spinal alignment, musculoskeletal health, and alternative healthcare", SkillCategory.Health),
            new("Elderly Care", "Senior health, aging support, caregiver training, and geriatric assistance", SkillCategory.Health)
        };
    }

    private static List<Skill> CreateCulinarySkills()
    {
        return new List<Skill>
        {
            new("Cooking", "Basic cooking techniques, recipe development, kitchen skills, and meal preparation", SkillCategory.Culinary),
            new("Baking", "Bread making, pastry techniques, cake decorating, and dessert preparation", SkillCategory.Culinary),
            new("Italian Cuisine", "Traditional Italian cooking, pasta making, regional specialties, and Italian techniques", SkillCategory.Culinary),
            new("Asian Cuisine", "Chinese, Japanese, Thai, Korean cooking techniques and authentic flavors", SkillCategory.Culinary),
            new("Pastry Making", "French pastries, croissants, tarts, chocolate work, and advanced baking", SkillCategory.Culinary),
            new("Vegetarian Cooking", "Plant-based meals, vegetarian nutrition, meat alternatives, and creative recipes", SkillCategory.Culinary),
            new("Grilling & BBQ", "Outdoor cooking, smoking techniques, meat preparation, and barbecue styles", SkillCategory.Culinary),
            new("Wine Tasting", "Wine appreciation, food pairing, vineyard knowledge, and sommelier skills", SkillCategory.Culinary),
            new("Meal Planning", "Menu development, nutrition balance, budget cooking, and batch preparation", SkillCategory.Culinary),
            new("Food Photography", "Culinary styling, food presentation, lighting, and restaurant photography", SkillCategory.Culinary),
            new("Fermentation", "Kombucha, kimchi, sourdough, cheese making, and fermented foods", SkillCategory.Culinary),
            new("Cake Decorating", "Fondant work, buttercream techniques, sugar art, and celebration cakes", SkillCategory.Culinary),
            new("Knife Skills", "Proper cutting techniques, knife maintenance, kitchen safety, and efficiency", SkillCategory.Culinary),
            new("Mexican Cuisine", "Traditional Mexican cooking, authentic recipes, spice blending, and regional dishes", SkillCategory.Culinary),
            new("French Cuisine", "Classical French techniques, mother sauces, fine dining, and culinary fundamentals", SkillCategory.Culinary),
            new("Cocktail Making", "Mixology, bartending, craft cocktails, spirits knowledge, and bar management", SkillCategory.Culinary),
            new("Food Safety", "HACCP principles, food handling, sanitation, and restaurant safety protocols", SkillCategory.Culinary),
            new("Chocolate Making", "Tempering, truffle making, confectionery, and artisan chocolate techniques", SkillCategory.Culinary),
            new("Preserving & Canning", "Food preservation, pickling, jam making, and food storage techniques", SkillCategory.Culinary),
            new("Catering", "Event catering, large-scale cooking, menu planning, and food service management", SkillCategory.Culinary)
        };
    }

    private static List<Skill> CreateCraftsSkills()
    {
        return new List<Skill>
        {
            new("Knitting", "Knitting patterns, techniques, yarn selection, and garment construction", SkillCategory.Crafts),
            new("Sewing", "Pattern making, garment construction, alterations, and sewing machine operation", SkillCategory.Crafts),
            new("Embroidery", "Hand embroidery, cross-stitch, decorative stitching, and needlework art", SkillCategory.Crafts),
            new("Crocheting", "Crochet patterns, techniques, amigurumi, and yarn crafts", SkillCategory.Crafts),
            new("Scrapbooking", "Memory keeping, photo albums, paper crafts, and creative layouts", SkillCategory.Crafts),
            new("Candle Making", "Wax selection, wick placement, scent blending, and candle crafting", SkillCategory.Crafts),
            new("Soap Making", "Cold process, hot process, natural ingredients, and soap crafting", SkillCategory.Crafts),
            new("Origami", "Paper folding, Japanese art, geometric designs, and paper sculptures", SkillCategory.Crafts),
            new("Quilting", "Patchwork, quilting patterns, fabric selection, and textile arts", SkillCategory.Crafts),
            new("Leatherworking", "Leather crafting, tooling, stitching, and leather goods making", SkillCategory.Crafts),
            new("Macrame", "Knotting techniques, wall hangings, plant hangers, and decorative macrame", SkillCategory.Crafts),
            new("Beadwork", "Jewelry making, beading patterns, wire work, and decorative beading", SkillCategory.Crafts),
            new("Card Making", "Handmade cards, paper crafts, stamping, and greeting card design", SkillCategory.Crafts),
            new("Model Making", "Scale models, miniatures, model trains, and detailed craftsmanship", SkillCategory.Crafts),
            new("Glass Blowing", "Glass art, vessel making, artistic glasswork, and furnace techniques", SkillCategory.Crafts),
            new("Basket Weaving", "Traditional weaving, natural materials, functional baskets, and fiber arts", SkillCategory.Crafts),
            new("Bookbinding", "Book repair, binding techniques, paper arts, and book crafting", SkillCategory.Crafts),
            new("Metal Working", "Welding, blacksmithing, jewelry making, and metalcraft techniques", SkillCategory.Crafts),
            new("Resin Art", "Epoxy resin, casting, resin jewelry, and artistic resin work", SkillCategory.Crafts),
            new("Upcycling", "Furniture restoration, repurposing materials, sustainable crafts, and creative reuse", SkillCategory.Crafts)
        };
    }

    private static List<Skill> CreateEducationSkills()
    {
        return new List<Skill>
        {
            new("Mathematics Tutoring", "Algebra, calculus, geometry, statistics, and mathematical problem solving", SkillCategory.Education),
            new("Science Teaching", "Biology, chemistry, physics, earth science, and scientific method instruction", SkillCategory.Education),
            new("Elementary Education", "Primary school teaching, child development, curriculum design, and classroom management", SkillCategory.Education),
            new("Special Education", "Learning disabilities, adaptive teaching, individualized education, and inclusive practices", SkillCategory.Education),
            new("Online Teaching", "Virtual classroom management, e-learning platforms, digital pedagogy, and remote instruction", SkillCategory.Education),
            new("Curriculum Development", "Educational design, learning objectives, assessment creation, and program planning", SkillCategory.Education),
            new("Adult Education", "Adult learning principles, professional development, continuing education, and skill training", SkillCategory.Education),
            new("Educational Technology", "Learning management systems, educational software, digital tools, and tech integration", SkillCategory.Education),
            new("Test Preparation", "SAT, GRE, TOEFL preparation, study strategies, and exam techniques", SkillCategory.Education),
            new("Early Childhood Education", "Preschool teaching, child development, play-based learning, and early literacy", SkillCategory.Education),
            new("Reading Instruction", "Phonics, reading comprehension, literacy development, and reading intervention", SkillCategory.Education),
            new("Writing Instruction", "Essay writing, creative writing, grammar, composition, and writing skills", SkillCategory.Education),
            new("History Teaching", "World history, American history, historical analysis, and social studies instruction", SkillCategory.Education),
            new("Art Education", "Art instruction, creativity development, art history, and visual arts teaching", SkillCategory.Education),
            new("Music Education", "Music theory, instrument instruction, choir direction, and music appreciation", SkillCategory.Education),
            new("Physical Education", "Sports instruction, fitness education, health education, and movement skills", SkillCategory.Education),
            new("Educational Assessment", "Testing, evaluation, rubric development, and learning measurement", SkillCategory.Education),
            new("Classroom Management", "Behavior management, student engagement, discipline strategies, and learning environment", SkillCategory.Education),
            new("Educational Psychology", "Learning theory, student motivation, cognitive development, and educational research", SkillCategory.Education),
            new("Homeschooling", "Home education, curriculum selection, parent teaching, and alternative education", SkillCategory.Education)
        };
    }

    private static List<Skill> CreateMusicSkills()
    {
        return new List<Skill>
        {
            new("Guitar Playing", "Acoustic and electric guitar, chord progressions, fingerpicking, and music theory", SkillCategory.Music),
            new("Piano Playing", "Classical and contemporary piano, music reading, technique, and performance", SkillCategory.Music),
            new("Violin", "Classical violin, bowing techniques, music reading, and orchestral performance", SkillCategory.Music),
            new("Singing", "Vocal technique, breath control, performance, and various musical styles", SkillCategory.Music),
            new("Drums", "Drum kit playing, rhythm patterns, coordination, and percussion techniques", SkillCategory.Music),
            new("Music Theory", "Harmony, scales, chord progressions, composition, and musical analysis", SkillCategory.Music),
            new("Song Writing", "Lyric writing, melody composition, song structure, and creative music creation", SkillCategory.Music),
            new("Music Recording", "Audio engineering, mixing, mastering, and studio recording techniques", SkillCategory.Music),
            new("Bass Guitar", "Electric bass playing, rhythm section, groove, and bass line creation", SkillCategory.Music),
            new("Saxophone", "Alto, tenor saxophone, jazz techniques, improvisation, and wind instruments", SkillCategory.Music),
            new("Flute", "Classical flute, breath control, embouchure, and woodwind techniques", SkillCategory.Music),
            new("Trumpet", "Brass instrument technique, range development, jazz, and classical performance", SkillCategory.Music),
            new("Cello", "String instrument technique, bowing, music reading, and chamber music", SkillCategory.Music),
            new("Electronic Music", "Synthesizers, MIDI, electronic composition, and digital music creation", SkillCategory.Music),
            new("DJ Skills", "Mixing, beatmatching, turntablism, and electronic music performance", SkillCategory.Music),
            new("Music Business", "Artist management, music marketing, copyright, and music industry knowledge", SkillCategory.Music),
            new("Conducting", "Orchestra conducting, choir direction, musical leadership, and ensemble management", SkillCategory.Music),
            new("Jazz Improvisation", "Jazz theory, improvisation techniques, bebop, and jazz performance", SkillCategory.Music),
            new("Classical Performance", "Classical music interpretation, technique, audition preparation, and concert performance", SkillCategory.Music),
            new("Music Therapy", "Therapeutic music application, healing through music, and clinical music practice", SkillCategory.Music)
        };
    }

    private static List<Skill> CreateSportsSkills()
    {
        return new List<Skill>
        {
            new("Tennis", "Tennis technique, strategy, coaching, and competitive play", SkillCategory.Sports),
            new("Basketball", "Basketball skills, team play, coaching, and athletic development", SkillCategory.Sports),
            new("Soccer", "Football skills, tactics, coaching, and team strategy", SkillCategory.Sports),
            new("Swimming", "Swimming techniques, stroke improvement, water safety, and competitive swimming", SkillCategory.Sports),
            new("Running", "Marathon training, sprinting, endurance building, and running technique", SkillCategory.Sports),
            new("Golf", "Golf swing, course management, putting, and golf instruction", SkillCategory.Sports),
            new("Baseball", "Baseball skills, pitching, batting, fielding, and team strategy", SkillCategory.Sports),
            new("Volleyball", "Volleyball techniques, team play, spiking, serving, and coaching", SkillCategory.Sports),
            new("Martial Arts", "Karate, taekwondo, judo, self-defense, and discipline training", SkillCategory.Sports),
            new("Rock Climbing", "Indoor and outdoor climbing, safety, technique, and equipment knowledge", SkillCategory.Sports),
            new("Cycling", "Road cycling, mountain biking, bike maintenance, and endurance training", SkillCategory.Sports),
            new("Skiing", "Alpine skiing, snowboarding, winter sports, and mountain safety", SkillCategory.Sports),
            new("Surfing", "Wave riding, ocean safety, surfboard technique, and water sports", SkillCategory.Sports),
            new("Fishing", "Angling techniques, fly fishing, boat fishing, and outdoor skills", SkillCategory.Sports),
            new("Hiking", "Trail navigation, outdoor survival, camping, and wilderness skills", SkillCategory.Sports),
            new("Boxing", "Boxing technique, training, fitness, and combat sports", SkillCategory.Sports),
            new("Gymnastics", "Gymnastics skills, flexibility, strength training, and athletic coaching", SkillCategory.Sports),
            new("Wrestling", "Wrestling techniques, grappling, strength training, and competitive wrestling", SkillCategory.Sports),
            new("Track and Field", "Sprint, distance, jumping, throwing events, and athletic training", SkillCategory.Sports),
            new("Crossfit", "Functional fitness, Olympic lifting, metabolic conditioning, and strength training", SkillCategory.Sports)
        };
    }

    private static List<Skill> CreateScienceSkills()
    {
        return new List<Skill>
        {
            new("Chemistry", "Organic, inorganic chemistry, laboratory techniques, and chemical analysis", SkillCategory.Science),
            new("Biology", "Molecular biology, genetics, ecology, and life sciences research", SkillCategory.Science),
            new("Physics", "Theoretical and applied physics, mechanics, thermodynamics, and quantum physics", SkillCategory.Science),
            new("Environmental Science", "Ecology, conservation, climate science, and environmental research", SkillCategory.Science),
            new("Astronomy", "Astrophysics, telescope operation, celestial observation, and space science", SkillCategory.Science),
            new("Geology", "Earth science, mineralogy, geological survey, and earth processes", SkillCategory.Science),
            new("Psychology", "Human behavior, cognitive psychology, research methods, and mental health", SkillCategory.Science),
            new("Neuroscience", "Brain science, neurological research, cognitive science, and neurobiology", SkillCategory.Science),
            new("Biotechnology", "Genetic engineering, biotech research, laboratory techniques, and bioinformatics", SkillCategory.Science),
            new("Marine Biology", "Ocean life, marine ecosystems, underwater research, and aquatic science", SkillCategory.Science),
            new("Meteorology", "Weather forecasting, climate analysis, atmospheric science, and weather systems", SkillCategory.Science),
            new("Archaeology", "Historical excavation, artifact analysis, cultural anthropology, and heritage studies", SkillCategory.Science),
            new("Anthropology", "Cultural studies, human evolution, social anthropology, and ethnographic research", SkillCategory.Science),
            new("Statistics", "Data analysis, statistical modeling, research design, and quantitative analysis", SkillCategory.Science),
            new("Research Methods", "Scientific methodology, experimental design, data collection, and analysis", SkillCategory.Science),
            new("Laboratory Techniques", "Scientific instrumentation, safety protocols, experimental procedures, and analysis", SkillCategory.Science),
            new("Botany", "Plant science, plant identification, horticulture, and plant biology", SkillCategory.Science),
            new("Zoology", "Animal behavior, animal biology, wildlife study, and veterinary science", SkillCategory.Science),
            new("Forensic Science", "Crime scene investigation, evidence analysis, forensic techniques, and legal science", SkillCategory.Science),
            new("Medical Science", "Healthcare research, medical knowledge, diagnostic techniques, and clinical science", SkillCategory.Science)
        };
    }

    private static List<Skill> CreateOtherSkills()
    {
        return new List<Skill>
        {
            new("Magic & Illusion", "Magic tricks, illusion performance, entertainment, and sleight of hand", SkillCategory.Other),
            new("Stand-up Comedy", "Comedy writing, performance, timing, and entertainment skills", SkillCategory.Other),
            new("Event Planning", "Wedding planning, corporate events, party organization, and event coordination", SkillCategory.Other),
            new("Gardening", "Plant care, landscaping, organic gardening, and horticulture", SkillCategory.Other),
            new("Pet Training", "Dog training, animal behavior, obedience training, and pet care", SkillCategory.Other),
            new("Home Repair", "DIY repairs, plumbing, electrical work, and home maintenance", SkillCategory.Other),
            new("Car Maintenance", "Auto repair, engine maintenance, troubleshooting, and automotive skills", SkillCategory.Other),
            new("Travel Planning", "Trip organization, budget travel, destination knowledge, and travel coordination", SkillCategory.Other),
            new("Personal Organization", "Time management, productivity, decluttering, and life organization", SkillCategory.Other),
            new("Etiquette", "Social skills, business etiquette, cultural awareness, and proper manners", SkillCategory.Other),
            new("Speed Reading", "Reading techniques, comprehension improvement, and information processing", SkillCategory.Other),
            new("Memory Techniques", "Memorization methods, mental training, cognitive enhancement, and learning strategies", SkillCategory.Other),
            new("Voice Acting", "Character voices, narration, audiobook recording, and vocal performance", SkillCategory.Other),
            new("Storytelling", "Narrative techniques, oral tradition, children's stories, and performance storytelling", SkillCategory.Other),
            new("Board Game Design", "Game mechanics, playtesting, game theory, and recreational game creation", SkillCategory.Other),
            new("Lockpicking", "Security analysis, lock mechanisms, puzzle solving, and security consulting", SkillCategory.Other),
            new("Meteorology Hobbyist", "Weather observation, storm chasing, climate tracking, and weather enthusiasm", SkillCategory.Other),
            new("Genealogy", "Family history research, ancestry tracing, historical records, and family tree construction", SkillCategory.Other),
            new("Amateur Radio", "HAM radio operation, emergency communication, electronics, and radio technology", SkillCategory.Other),
            new("Life Coaching", "Personal development, goal setting, motivation, and life guidance", SkillCategory.Other)
        };
    }
}
