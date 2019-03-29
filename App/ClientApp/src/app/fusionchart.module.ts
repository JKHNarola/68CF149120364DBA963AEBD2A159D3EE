import { FusionChartsModule } from 'angular-fusioncharts';
import FusionCharts from 'fusioncharts/core';
import Column2D from 'fusioncharts/viz/column2d';
import Doughnut2d from 'fusioncharts/viz/doughnut2d';
import MsLine from 'fusioncharts/viz/msline';
import Bubble from 'fusioncharts/viz/bubble';
import * as FusionTheme from 'fusioncharts/themes/fusioncharts.theme.fusion';
import * as CandyTheme from 'fusioncharts/themes/fusioncharts.theme.candy';
import * as CarbonTheme from 'fusioncharts/themes/fusioncharts.theme.carbon';
import * as GammelTheme from 'fusioncharts/themes/fusioncharts.theme.gammel';
import * as OceanTheme from 'fusioncharts/themes/fusioncharts.theme.ocean';
import * as ZuneTheme from 'fusioncharts/themes/fusioncharts.theme.zune';
import * as FintTheme from 'fusioncharts/themes/fusioncharts.theme.fint';
import { NgModule } from '@angular/core';
FusionChartsModule.fcRoot(
    FusionCharts,
    Column2D,
    Doughnut2d,
    MsLine,
    Bubble,
    FusionTheme,
    CandyTheme,
    GammelTheme,
    CarbonTheme,
    OceanTheme,
    ZuneTheme,
    FintTheme);

@NgModule({
    declarations: [],
    imports: [
        FusionChartsModule
    ],
    exports: [FusionChartsModule]
})
export class FusionChartModule { }
