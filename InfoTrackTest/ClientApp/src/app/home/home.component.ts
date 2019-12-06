import { Component, Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['home.css']
})
export class HomeComponent {
    public response: IResponse;
    public loading: boolean;
    private _http: HttpClient;
    private _baseUrl: string;
    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this._http = http;
        this._baseUrl = baseUrl;
        this.loading = false;
    }

    check() {
      this.loading = true;
        this._http.get<IResponse>(this._baseUrl + "search").subscribe(
            res => { this.response = res }, error => console.error(error)
        );
        this.loading = false;
    }
}

interface IResult {
    title: string;
    index:number;
}

interface IResponse {
    success: boolean;
    message: string;
    results: IResult[];
}
