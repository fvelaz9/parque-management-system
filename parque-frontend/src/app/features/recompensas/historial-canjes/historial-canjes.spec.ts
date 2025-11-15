import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HistorialCanjes } from './historial-canjes';

describe('HistorialCanjes', () => {
  let component: HistorialCanjes;
  let fixture: ComponentFixture<HistorialCanjes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HistorialCanjes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HistorialCanjes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
