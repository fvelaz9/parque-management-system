import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecompensasList } from './recompensas-list';

describe('RecompensasList', () => {
  let component: RecompensasList;
  let fixture: ComponentFixture<RecompensasList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecompensasList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecompensasList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
