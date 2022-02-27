import { Component, Inject, ViewChild } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { GoogleMap, GoogleMapsModule } from "@angular/google-maps";
import * as xlsx from "xlsx";
import pdfMake from "pdfmake/build/pdfmake";
import pdfFonts from "pdfmake/build/vfs_fonts";
import { ChartConfiguration, Chart, ChartType } from "chart.js";
import { BaseChartDirective } from "ng2-charts";
pdfMake.vfs = pdfFonts.pdfMake.vfs;

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["home.scss"],
})
export class HomeComponent {
  public response: IResponse;
  public archiStarResponse: IArchistartResult;
  public planningResponse: IPlanningResponse;

  public loading: boolean;
  private _http: HttpClient;
  private _baseUrl: string;
  private convertJson: string;
  private apiKey = "C1xvLXs4X17oJKAUNUNw9ak4U4cpQLB5quqDcpeh";
  private streetView: google.maps.StreetViewPanorama;
  private dummybody = {
    streetAddress: "1-7 Castlereagh Street",
    suburb: "Sydney",
    postCode: 2000,
  };

  constructor(http: HttpClient, @Inject("BASE_URL") baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
    this.loading = false;
  }

  public lineChartData: ChartConfiguration["data"] = {
    datasets: [
      {
        data: [65, 59, 80, 81, 56, 55, 40],
        label: "Series A",
        backgroundColor: "rgba(148,159,177,0.2)",
        borderColor: "rgba(148,159,177,1)",
        pointBackgroundColor: "rgba(148,159,177,1)",
        pointBorderColor: "#fff",
        pointHoverBackgroundColor: "#fff",
        pointHoverBorderColor: "rgba(148,159,177,0.8)",
        fill: "origin",
      },
      {
        data: [28, 48, 40, 19, 86, 27, 90],
        label: "Series B",
        backgroundColor: "rgba(77,83,96,0.2)",
        borderColor: "rgba(77,83,96,1)",
        pointBackgroundColor: "rgba(77,83,96,1)",
        pointBorderColor: "#fff",
        pointHoverBackgroundColor: "#fff",
        pointHoverBorderColor: "rgba(77,83,96,1)",
        fill: "origin",
      },
      {
        data: [180, 480, 770, 90, 1000, 270, 400],
        label: "Series C",
        yAxisID: "y-axis-1",
        backgroundColor: "rgba(255,0,0,0.3)",
        borderColor: "red",
        pointBackgroundColor: "rgba(148,159,177,1)",
        pointBorderColor: "#fff",
        pointHoverBackgroundColor: "#fff",
        pointHoverBorderColor: "rgba(148,159,177,0.8)",
        fill: "origin",
      },
    ],
    labels: ["January", "February", "March", "April", "May", "June", "July"],
  };

  public lineChartOptions: ChartConfiguration["options"] = {
    elements: {
      line: {
        tension: 0.5,
      },
    },
    scales: {
      // We use this empty structure as a placeholder for dynamic theming.
      // x: {},
      // 'y-axis-0':
      //   {
      //     position: 'left',
      //   },
      // 'y-axis-1': {
      //   position: 'right',
      //   grid: {
      //     color: 'rgba(255,0,0,0.3)',
      //   },
      //   ticks: {
      //     color: 'red'
      //   }
      // }
    },

    plugins: {
      legend: { display: true },
      annotation: {
        annotations: [
          {
            type: "line",
            scaleID: "x",
            value: "March",
            borderColor: "orange",
            borderWidth: 2,
            label: {
              position: "center",
              enabled: true,
              color: "orange",
              content: "LineAnno",
              font: {
                weight: "bold",
              },
            },
          },
        ],
      },
    },
  };

 
  // events


 

  public lineChartType: ChartType = "line";

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  @ViewChild(GoogleMap) map!: GoogleMap;

  center: google.maps.LatLngLiteral;
  generatePDF() {
    let docDefinition = {
      header: "C#Corner PDF Header",
      content:
        "Sample PDF generated with Angular and PDFMake for C#Corner Blog",
    };

    pdfMake.createPdf(docDefinition).open();
  }

  fileUpload(event: any) {
    console.log(event.target.files);
    const selectedFile = event.target.files[0];

    const fileReader = new FileReader();

    fileReader.readAsBinaryString(selectedFile);
    fileReader.onload = (event) => {
      console.log(event);
      let binaryData = event.target.result;
      let workbook = xlsx.read(binaryData, { type: "binary", sheetRows: 14 });
      console.log(workbook);
      // setTimeout(function () {

      // }, 5000);

      var sn = workbook.SheetNames[0];

      const data = xlsx.utils.sheet_to_json(workbook.Sheets[sn]);

      this.convertJson = JSON.stringify(data);
      console.log(this.convertJson);
    };
  }

  ngAfterViewInit() {
    // this.streetView = this.map.getStreetView();
    // this.streetView.setOptions({
    //    position: { lat: 38.9938386, lng: -77.2515373 },
    //    pov: { heading: 70, pitch: -10 },
    // });
    // this.streetView.setVisible(true);
  }

  check() {
    this.loading = true;
    this._http.get<IResponse>(this._baseUrl + "search").subscribe(
      (res) => {
        this.response = res;
      },
      (error) => console.error(error)
    );
    this.loading = false;
  }

  fetchArchistarSuggest(street: string, city: string, postCode: number) {
    this.loading = true;
    const headers = {
      "content-type": "application/json",
      "x-api-key": this.apiKey,
    };
    const body = JSON.stringify({
      streetAddress: street,
      suburb: city,
      postCode: Number.parseInt(postCode.toString()),
    });
    this._http
      .post<IArchistartResult>(
        "https://api.archistar.ai/v1/property/suggest",
        body,
        { headers: headers }
      )
      .subscribe(
        (res) => {
          alert("result successfully fetched from archistart ai");
          this.archiStarResponse = res;
          console.log(res);
          this._http
            .post<IPlanningResponse>(
              "https://api.archistar.ai/v1/property/planning-essentials",
              {
                propertyID: this.archiStarResponse.result.propertyID,
              },
              { headers: headers }
            )
            .subscribe(
              (res) => {
                this.planningResponse = res;
                console.log(res);
                console.log(
                  this.planningResponse.result.Property.property_details
                    .center_lat,
                  this.planningResponse.result.Property.property_details
                    .center_long
                );

                // -33.86620630039655, 151.20977064237633
                this.center = {
                  lat: Number.parseInt(
                    this.planningResponse.result.Property.property_details
                      .center_lat
                  ),
                  lng: Number.parseInt(
                    this.planningResponse.result.Property.property_details
                      .center_long
                  ),
                };
                this.center = {
                  lat: -33.86620630039655,
                  lng: 151.20977064237633,
                };

                // this.streetView.setOptions({
                //   position: {
                //     lat: Number.parseInt(
                //       this.planningResponse.result.Property.property_details.center_lat.substring(
                //         0,
                //         8
                //       )
                //     ),
                //     lng: Number.parseInt(
                //       this.planningResponse.result.Property.property_details.center_long.substring(
                //         0,
                //         8
                //       )
                //     ),
                //   },
                //   pov: { heading: 70, pitch: -10 },
                // });

                // console.log(this.streetView);
                // this.streetView.setVisible(true);
              },
              (error) => console.error(error)
            );
        },
        (error) => console.error(error)
      );
    this.loading = false;
  }
}

interface IResult {
  title: string;
  index: number;
}

interface IArchistartResult {
  result: { propertyID: string; listingAddress: string; regionID: string };
}

interface IResponse {
  success: boolean;
  message: string;
  results: IResult[];
}

export interface PropertyDetails {
  lot_identifier: string;
  localities: any[];
  property_type: string;
  region: string;
  center_lat: string;
  center_long: string;
  property_area_m2: string;
  arr_gnaf_address_detail_pid: string[];
}

export interface Zoning {
  code: string;
  label: string;
}

export interface Overlays {
  overlay_id: string;
  category: string;
  key: string;
  value: string;
}

export interface PermittedWithConsent {
  value: string;
}

export interface PermittedWithoutConsent {
  value: string;
}

export interface Prohibited {
  value: string;
}

export interface ArrNotificationsPermittedUse {
  Permittedwithconsent: PermittedWithConsent[];
  Permittedwithoutconsent: PermittedWithoutConsent[];
  Prohibited: Prohibited[];
}

export interface PropertyPlanningDetails {
  zoning: Zoning;
  overlays: Overlays;
  lga: string;
  has_flood_overlay: boolean;
  has_bushfire_overlay: boolean;
  has_infrastructure_overlay: boolean;
  has_environment_overlay: boolean;
  has_heritage_overlay: boolean;
  has_industry_overlay: boolean;
  arr_notifications_permitted_use: ArrNotificationsPermittedUse;
}

export interface PermittedUseDuplexRuleMessage {
  message: string;
  url?: any;
}

export interface PropertyAnalysis {
  permitted_use_apartment?: any;
  permitted_use_duplex?: any;
  permitted_use_duplex_rule_message: PermittedUseDuplexRuleMessage[];
  permitted_use_mixeduse: boolean;
  permitted_use_townhouse?: any;
}

export interface Property {
  property_details: PropertyDetails;
  property_planning_details: PropertyPlanningDetails;
  property_analysis: PropertyAnalysis;
  propertyID: string;
}

export interface Result {
  Property: Property;
}

export interface IPlanningResponse {
  result: Result;
}
