import { Component, NgModule } from '@angular/core';
import { GoogleMapsModule } from '@angular/google-maps';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

@NgModule({
  imports:[GoogleMapsModule]
})
export class AppComponent {
  title = 'app';
}
