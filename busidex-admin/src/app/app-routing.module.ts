import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AddCardComponent } from './add-card/add-card.component';
import { UnownedCardsListComponent } from './unowned-cards-list/unowned-cards-list.component';
import { CardDetailComponent } from './card-detail/card-detail.component';


const routes: Routes = [
  { path: 'add-card', component: AddCardComponent },
  { path: 'card-detail', component: CardDetailComponent },
  { path: 'unowned-cards', component: UnownedCardsListComponent },
  { path: '', redirectTo: '/unowned-cards', pathMatch: 'full' },
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
