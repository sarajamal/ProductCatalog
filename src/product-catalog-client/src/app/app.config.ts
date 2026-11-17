//الربط مع الـ API: الـ Service
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient() //her بدون هذا السطر ما يقدر Angular يرسل أي طلب HTTP
  ]
};
