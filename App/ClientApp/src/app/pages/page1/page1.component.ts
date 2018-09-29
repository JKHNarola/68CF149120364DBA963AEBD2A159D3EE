import { Component, ElementRef, ViewChild } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { PrintService } from '../../services/print.service';

@Component({
    selector: 'page-one',
    templateUrl: './page1.component.html',
    animations: [pageSlideUpAnimation],
    providers: [PrintService]
})
export class Page1Component {
    dataSourceColumn2D: Object;
    dataSourceDoughnut2D: Object;
    dataSourceMultiLine2D: Object;
    dataSourceBubble: Object;

    @ViewChild('printSection') printEl: ElementRef;
    constructor(private printingService: PrintService) {
        this.dataSourceColumn2D = {
            chart: {
                "caption": "Countries With Most Oil Reserves [2017-18]",
                "subCaption": "In MMbbl = One Million barrels",
                "xAxisName": "Country",
                "yAxisName": "Reserves (MMbbl)",
                "numberSuffix": "K",
                "theme": "fusion"
            },
            // Chart Data
            "data": [{
                "label": "Venezuela",
                "value": "290"
            }, {
                "label": "Saudi",
                "value": "260"
            }, {
                "label": "Canada",
                "value": "180"
            }, {
                "label": "Iran",
                "value": "140"
            }, {
                "label": "Russia",
                "value": "115"
            }, {
                "label": "UAE",
                "value": "100"
            }, {
                "label": "US",
                "value": "30"
            }, {
                "label": "China",
                "value": "30"
            }]
        };

        this.dataSourceDoughnut2D = {
            "chart": {
                "caption": "Android Distribution for our app",
                "subcaption": "For all users in 2017",
                "showpercentvalues": "1",
                "defaultcenterlabel": "Android Distribution",
                "aligncaptionwithcanvas": "0",
                "captionpadding": "0",
                "decimals": "1",
                "plottooltext": "<b>$percentValue</b> of our Android users are on <b>$label</b>",
                "centerlabel": "# Users: $value",
                "theme": "fusion"
            },
            "data": [
                {
                    "label": "Ice Cream Sandwich",
                    "value": "1000"
                },
                {
                    "label": "Jelly Bean",
                    "value": "5300"
                },
                {
                    "label": "Kitkat",
                    "value": "10500"
                },
                {
                    "label": "Lollipop",
                    "value": "18900"
                },
                {
                    "label": "Marshmallow",
                    "value": "17904"
                }
            ]
        }

        this.dataSourceMultiLine2D = {
            "chart": {
                "caption": "Reach of Social Media Platforms amoung youth",
                "yaxisname": "% of youth on this platform",
                "subcaption": "2012-2016",
                "showhovereffect": "1",
                "numbersuffix": "%",
                "drawcrossline": "1",
                "plottooltext": "<b>$dataValue</b> of youth were on $seriesName",
                "theme": "fusion"
            },
            "categories": [
                {
                    "category": [
                        {
                            "label": "2012"
                        },
                        {
                            "label": "2013"
                        },
                        {
                            "label": "2014"
                        },
                        {
                            "label": "2015"
                        },
                        {
                            "label": "2016"
                        }
                    ]
                }
            ],
            "dataset": [
                {
                    "seriesname": "Facebook",
                    "data": [
                        {
                            "value": "62"
                        },
                        {
                            "value": "64"
                        },
                        {
                            "value": "64"
                        },
                        {
                            "value": "66"
                        },
                        {
                            "value": "78"
                        }
                    ]
                },
                {
                    "seriesname": "Instagram",
                    "data": [
                        {
                            "value": "16"
                        },
                        {
                            "value": "28"
                        },
                        {
                            "value": "34"
                        },
                        {
                            "value": "42"
                        },
                        {
                            "value": "54"
                        }
                    ]
                },
                {
                    "seriesname": "LinkedIn",
                    "data": [
                        {
                            "value": "20"
                        },
                        {
                            "value": "22"
                        },
                        {
                            "value": "27"
                        },
                        {
                            "value": "22"
                        },
                        {
                            "value": "29"
                        }
                    ]
                },
                {
                    "seriesname": "Twitter",
                    "data": [
                        {
                            "value": "18"
                        },
                        {
                            "value": "19"
                        },
                        {
                            "value": "21"
                        },
                        {
                            "value": "21"
                        },
                        {
                            "value": "24"
                        }
                    ]
                }
            ]
        }

        this.dataSourceBubble = {
            "chart": {
                "caption": "AdWords Campaign Analysis",
                "subcaption": "Conversions as % of total",
                "xaxisname": "# Conversions",
                "yaxisname": "Cost Per Conversion",
                "numberprefix": "$",
                "theme": "fusion",
                "plottooltext": "$name : Share of total conversion: $zvalue%"
            },
            "categories": [
                {
                    "verticallinealpha": "20",
                    "category": [
                        {
                            "label": "0",
                            "x": "0"
                        },
                        {
                            "label": "1500",
                            "x": "1500",
                            "showverticalline": "1"
                        },
                        {
                            "label": "3000",
                            "x": "3000",
                            "showverticalline": "1"
                        },
                        {
                            "label": "4500",
                            "x": "4500",
                            "showverticalline": "1"
                        },
                        {
                            "label": "6000",
                            "x": "6000",
                            "showverticalline": "1"
                        }
                    ]
                }
            ],
            "dataset": [
                {
                    "data": [
                        {
                            "x": "5540",
                            "y": "16.09",
                            "z": "30.63",
                            "name": "Campaign 1"
                        },
                        {
                            "x": "4406",
                            "y": "12.74",
                            "z": "24.36",
                            "name": "Campaign 2"
                        },
                        {
                            "x": "1079",
                            "y": "15.79",
                            "z": "5.97",
                            "name": "Campaign 3"
                        },
                        {
                            "x": "1700",
                            "y": "8.27",
                            "z": "9.4",
                            "name": "Campaign 4"
                        },
                        {
                            "x": "853",
                            "y": "15.89",
                            "z": "4.71",
                            "name": "Campaign 5"
                        },
                        {
                            "x": "1202",
                            "y": "10.74",
                            "z": "6.65",
                            "name": "Campaign 6"
                        },
                        {
                            "x": "2018",
                            "y": "6.14",
                            "z": "11.16",
                            "name": "Campaign 7"
                        },
                        {
                            "x": "413",
                            "y": "19.83",
                            "z": "2.28",
                            "name": "Campaign 8"
                        },
                        {
                            "x": "586",
                            "y": "13.96",
                            "z": "3.24",
                            "name": "Campaign 9"
                        },
                        {
                            "x": "184",
                            "y": "15.82",
                            "z": "1.02",
                            "name": "Campaign 10"
                        },
                        {
                            "x": "311",
                            "y": "5.83",
                            "z": "1.72",
                            "name": "Campaign 11"
                        },
                        {
                            "x": "35",
                            "y": "10.76",
                            "z": "0.19",
                            "name": "Campaign 12"
                        },
                        {
                            "x": "55",
                            "y": "2.73",
                            "z": "0.3",
                            "name": "Campaign 13"
                        },
                        {
                            "x": "6",
                            "y": "21.22",
                            "z": "0.03",
                            "name": "Campaign 14"
                        }
                    ]
                }
            ]
        }
    }

    onPrint() {
        this.printingService.print(this.printEl.nativeElement);
    }
}
