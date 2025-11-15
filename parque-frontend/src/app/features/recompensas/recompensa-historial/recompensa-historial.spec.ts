import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecompensaHistorial } from './recompensa-historial';

describe('RecompensaHistorial', () => {
  let component: RecompensaHistorial;
  let fixture: ComponentFixture<RecompensaHistorial>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecompensaHistorial]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecompensaHistorial);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
