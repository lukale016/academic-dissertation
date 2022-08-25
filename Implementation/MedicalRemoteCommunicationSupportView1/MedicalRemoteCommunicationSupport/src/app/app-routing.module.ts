import { AppointmentsComponent } from './appointments/appointments.component';
import { DoctorOverviewComponent } from './doctor-overview/doctor-overview.component';
import { TopicComponent } from './topic/topic/topic.component';
import { LoginComponent } from './auth/login/login.component';
import { HomeComponent } from './home/home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  {
    path: "login",
    component: LoginComponent
  },
  {
    path: "register",
    component: RegisterComponent
  },
  {
    path: "doctor-overview",
    component: DoctorOverviewComponent
  },
  {
    path: "appointments",
    component: AppointmentsComponent
  },
  {
    path: "topic/:id",
    component: TopicComponent
  },
  {
    path: "",
    component: HomeComponent
  },
  {
    path: "**",
    redirectTo: ""
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
