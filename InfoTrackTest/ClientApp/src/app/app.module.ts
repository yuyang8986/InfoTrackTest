import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgChartsModule } from 'ng2-charts';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { GoogleMapsModule } from '@angular/google-maps'

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        // GoogleMapsModule
    ],
    imports: [
        HttpClientModule,
        GoogleMapsModule,
        BrowserModule,
        NgChartsModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' }
        ])
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
