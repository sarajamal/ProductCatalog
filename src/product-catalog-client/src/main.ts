import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));

  
  /*ﻧﻘﻄﺔ اﻧﻄﻼق اﻟﺘﻄﺒﻴﻖ — أول ﻣﻠﻒ ﻳﺸﺘﻐﻞ، ﻳﺸﻐّﻞ

.Program.cs ﻳﻘﺎﺑﻠﻬﺎ ﺑﺎﻟﺒﺎك اﻧﺪ ﻣﻠﻒ .AppComponent
*/